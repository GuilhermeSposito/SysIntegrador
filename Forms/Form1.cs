using System.Net.Http.Headers;
using System.Security.Policy;
using System.Text.Json;
using ExCSS;
using Microsoft.EntityFrameworkCore;
using SysIntegradorApp.ClassesAuxiliares;
using System.Diagnostics;
using System.Windows.Forms;
using SysIntegradorApp.data;
using System.Drawing.Drawing2D;
using SysIntegradorApp.Forms;
using SysIntegradorApp.ClassesDeConexaoComApps;


namespace SysIntegradorApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            ClsEstiloComponentes.SetRoundedRegion(majorPanel, 24);
            ClsEstiloComponentes.SetRoundedRegion(pictureBoxSysLogica, 24);
            ClsEstiloComponentes.SetRoundedRegion(CodeFromUser, 24);

            CodeFromUser.Height = 500;
            groupBoxAut.Visible = false;
            CodeFromUser.Visible = false;
            labelCodigo.Visible = false;
        }
        private async void BrnAutorizar_Click(object sender, EventArgs e)
        {
            string url = "https://merchant-api.ifood.com.br/authentication/v1.0/oauth/";
            try
            {
                using ApplicationDbContext db = new ApplicationDbContext();
                ParametrosDoSistema? opcSistema = db.parametrosdosistema.ToList().FirstOrDefault();

                string? codeFromMenu = CodeFromUser.Text;

                FormUrlEncodedContent formDataToGetTheToken = new FormUrlEncodedContent(new[]
                     {
                        new KeyValuePair<string, string>("grantType", "authorization_code"),
                        new KeyValuePair<string, string>("clientId", opcSistema.ClientId),
                        new KeyValuePair<string, string>("clientSecret", opcSistema.ClientSecret),
                        new KeyValuePair<string, string>("authorizationCode", codeFromMenu),
                        new KeyValuePair<string, string>("authorizationCodeVerifier", UserCodeReturnFromAPI.CodeVerifier)
                });

                using HttpClient client = new HttpClient();

                HttpResponseMessage responseWithToken = await client.PostAsync($"{url}token", formDataToGetTheToken);
                if (!responseWithToken.IsSuccessStatusCode)
                {
                    throw new HttpRequestException("\nErro ao acessar o token de acesso\n");
                }

                string? jsonObjTokenFromAPI = await responseWithToken.Content.ReadAsStringAsync();
                Token propriedadesAPIWithToken = JsonSerializer.Deserialize<Token>(jsonObjTokenFromAPI);

                DateTime horaAtual = DateTime.Now;
                double milissegundosAdicionais = 21600;
                DateTime horaFutura = horaAtual.AddSeconds(propriedadesAPIWithToken.expiresIn);
                string HoraFormatada = horaFutura.ToString();

                propriedadesAPIWithToken.VenceEm = HoraFormatada;

                var tokenDB = db.parametrosdeautenticacao.ToList().FirstOrDefault();

                if (tokenDB == null)
                { // se entrar aqui é porque não existe nenhum token no banco de dados
                    db.parametrosdeautenticacao.Add(propriedadesAPIWithToken);
                    db.SaveChanges();
                }
                else
                { //entra aqui e atualiza as informações dentro do banco de dados para o novo token, e não cria um novo no banco de dados, só atualiza
                    tokenDB.accessToken = propriedadesAPIWithToken.accessToken;
                    tokenDB.refreshToken = propriedadesAPIWithToken.refreshToken;
                    tokenDB.expiresIn = propriedadesAPIWithToken.expiresIn;
                    tokenDB.VenceEm = HoraFormatada;
                    db.SaveChanges();
                }

                Token.TokenDaSessao = propriedadesAPIWithToken.accessToken;

                FormMenuInicial menu = new FormMenuInicial();
                menu.Show();
                this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ops", MessageBoxButtons.OKCancel);
            }
        }

        private void CodeFromUser_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                BrnAutorizar_Click(sender, EventArgs.Empty);
            }
        }


        private async void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                using ApplicationDbContext db = new ApplicationDbContext();
                var AutenticacaoNaBase = db.parametrosdeautenticacao.ToList().FirstOrDefault();
                var ConfigApp = db.parametrosdosistema.ToList().FirstOrDefault();

                if (ConfigApp.IntegraIfood)
                {
                    await Ifood.RefreshTokenIfood();

                    ParametrosDoSistema? ConfigSistem = db.parametrosdosistema.ToList().FirstOrDefault();

                    string idMerchant = ConfigSistem.MerchantId;
                    string url = $"https://merchant-api.ifood.com.br/merchant/v1.0/merchants/{idMerchant}/status";


                    Token? tokenNoDb = db.parametrosdeautenticacao.ToList().FirstOrDefault();
                    ParametrosDoSistema? Config = db.parametrosdosistema.FirstOrDefault();

                    if (Config.IntegracaoSysMenu)
                    {
                        bool CaixaAberto = ClsDeIntegracaoSys.VerificaCaixaAberto();

                        if (!CaixaAberto)
                        {
                            MessageBox.Show("Seu app está integrado com o SysMenu, Por favor abra o caixa antes de continuar", "Caixa Fechado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            this.Dispose();
                        }

                    }

                    using HttpClient client = new HttpClient();
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenNoDb.accessToken);
                    HttpResponseMessage response = await client.GetAsync(url);

                    if ((int)response.StatusCode != 401)
                    {
                        Token.TokenDaSessao = tokenNoDb.accessToken;

                        FormMenuInicial menu = new FormMenuInicial();
                        menu.Show();
                        this.Hide();
                    }

                }
                else
                {
                    FormMenuInicial menu = new FormMenuInicial();
                    menu.Show();
                    this.Hide();
                }

                if (ConfigApp.IntegraDelMatch)
                {
                    await DelMatch.GetToken();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Ops");
            }


        }

        private async void pictureBoxCadeado_Click(object sender, EventArgs e)
        {
            try
            {
                panelInstrucoes.Visible = false;

                using ApplicationDbContext db = new ApplicationDbContext();
                ParametrosDoSistema? opcSistema = db.parametrosdosistema.ToList().FirstOrDefault();

                CodeFromUser.Visible = true;
                labelCodigo.Visible = true;
                groupBoxAut.Visible = true;

                string url = "https://merchant-api.ifood.com.br/authentication/v1.0/oauth/";

                using (HttpClient client = new HttpClient())
                {
                    FormUrlEncodedContent formData = new FormUrlEncodedContent(new[]
                        {
                        new KeyValuePair<string, string>("clientId", opcSistema.ClientId)
                         });

                    HttpResponseMessage response = await client.PostAsync($"{url}userCode", formData);
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new HttpRequestException("\nErro ao acessar o user code\n");
                    }

                    string jsonContent = await response.Content.ReadAsStringAsync();
                    UserCodeReturnFromAPI codesOfVerif = JsonSerializer.Deserialize<UserCodeReturnFromAPI>(jsonContent);
                    UserCodeReturnFromAPI.CodeVerifier = codesOfVerif.authorizationCodeVerifier; //seta o valor em uma propriedade static para podermos pegar no proximo método 

                    string? urlVerificacao = codesOfVerif.verificationUrlComplete;

                    if (urlVerificacao != null && Uri.IsWellFormedUriString(urlVerificacao, UriKind.Absolute))
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = urlVerificacao,
                            UseShellExecute = true
                        });
                    }

                    Clipboard.SetText(codesOfVerif.userCode);

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ops", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void pictureBoxInfo_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Caso nescessite de mais ajuda contatar a SysLogica", "Informações", MessageBoxButtons.OK);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (Clipboard.ContainsText())
            {
                CodeFromUser.Text = Clipboard.GetText();
            }
            else
            {
                MessageBox.Show("Area de transferencia Vazia", "Ops");
            }
        }

        private void pictureBoxDeColar_Click(object sender, EventArgs e)
        {
            if (Clipboard.ContainsText())
            {
                CodeFromUser.Text = Clipboard.GetText();
            }
            else
            {
                MessageBox.Show("Area de transferencia Vazia", "Ops");
            }
        }

        private void panelDeColar_Paint(object sender, PaintEventArgs e) { }

        private void panelDeColar_Click(object sender, EventArgs e)
        {
            if (Clipboard.ContainsText())
            {
                CodeFromUser.Text = Clipboard.GetText();
            }
            else
            {
                MessageBox.Show("Area de transferencia Vazia", "Ops");
            }
        }
    }



}
