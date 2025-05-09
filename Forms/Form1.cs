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
using SysIntegradorApp.ClassesAuxiliares.Verificacoes;
using SysIntegradorApp.ClassesAuxiliares.logs;
using System.Runtime.Intrinsics.Arm;
using SysIntegradorApp.Forms.ONPEDIDO;
using SysIntegradorApp.data.InterfaceDeContexto;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.ComponentModel;
using Microsoft.VisualBasic.Logging;
using System.Configuration.Internal;


namespace SysIntegradorApp
{
    public partial class Form1 : Form
    {
        public readonly IMeuContexto _context;
        private readonly FormMenuInicial _formMenuInicial;

        public Form1(IMeuContexto context, FormMenuInicial formMenuInicial)
        {
            _context = context;
            _formMenuInicial = formMenuInicial;

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
                //using ApplicationDbContext db = new ApplicationDbContext();
                using (ApplicationDbContext db = await _context.GetContextoAsync())
                {
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
                    { // se entrar aqui � porque n�o existe nenhum token no banco de dados
                        db.parametrosdeautenticacao.Add(propriedadesAPIWithToken);
                        db.SaveChanges();
                    }
                    else
                    { //entra aqui e atualiza as informa��es dentro do banco de dados para o novo token, e n�o cria um novo no banco de dados, s� atualiza
                        tokenDB.accessToken = propriedadesAPIWithToken.accessToken;
                        tokenDB.refreshToken = propriedadesAPIWithToken.refreshToken;
                        tokenDB.expiresIn = propriedadesAPIWithToken.expiresIn;
                        tokenDB.VenceEm = HoraFormatada;
                        db.SaveChanges();
                    }

                    Token.TokenDaSessao = propriedadesAPIWithToken.accessToken;

                    var menu = _formMenuInicial;
                    menu.Show();
                    this.Hide();
                }

            }
            catch (Exception ex)
            {
                await Logs.CriaLogDeErro(ex.ToString());
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
                notifyIcon1.Visible = true;
                notifyIcon1.Text = "Bem vindo ao sistema de integra��o de Aplicativos de delivery Syslogica!";
                notifyIcon1.ShowBalloonTip(300, "Bem-Vindo", "Bem vindo ao sistema de integra��o de Aplicativos de delivery Syslogica!", ToolTipIcon.Info);
                notifyIcon1.Dispose();

                bool verificaInternet = await VerificaInternet.InternetAtiva();

                if (!verificaInternet)
                {
                    ClsSons.PlaySom2();
                    MessageBox.Show("Por favor verifique sua conex�o com a internet", "Aten��o", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    ClsSons.StopSom();
                    Application.Exit();
                }

                using (ApplicationDbContext db = await _context.GetContextoAsync())
                {
                    var AutenticacaoNaBase = db.parametrosdeautenticacao.FirstOrDefault();
                    var ConfigApp = db.parametrosdosistema.FirstOrDefault();

                    if (ConfigApp.IntegraGarcom)
                    {
                        //se integrar gar�om chama a fun��o para atualizar banco de dados
                        GarcomSysMenu garcomSysMenu = new GarcomSysMenu(new MeuContexto());

                        await garcomSysMenu.AtualizarBancoDeDadosParaOGarcon();

                    }

                    if (ConfigApp.IntegraDelMatch)
                    {
                        DelMatch Delmatch = new DelMatch(new MeuContexto());

                        await Delmatch.GetToken();
                    }

                    if (ConfigApp.IntegraOnOPedido)
                    {
                        OnPedido OnPedido = new OnPedido(new MeuContexto());

                        DateTime HorarioAtualDoToken = DateTime.Parse(AutenticacaoNaBase.VenceEmOnPedido);

                        if (DateTime.Now > HorarioAtualDoToken)
                        {
                            await OnPedido.GetToken();
                        }
                    }

                    if (ConfigApp.IntegraIfood)
                    {
                        ParametrosDoSistema? ConfigSistem = db.parametrosdosistema.FirstOrDefault();
                        if (ConfigSistem!.IfoodMultiEmpresa)
                        {
                            var Empresas = await db.empresasIfoods.ToListAsync();
                            foreach (var empresa in Empresas)
                            {
                                if (empresa.Online)
                                {
                                    empresa.Online = false;
                                    await db.SaveChangesAsync();
                                }
                            }
                        }

                        Ifood Ifood = new Ifood(new MeuContexto());
                        await Ifood.RefreshTokenIfood();


                        Token? tokenNoDb = await db.parametrosdeautenticacao.FirstOrDefaultAsync();

                        if (ConfigSistem.IntegracaoSysMenu)
                        {
                            bool CaixaAberto = await ClsDeIntegracaoSys.VerificaCaixaAberto();

                            if (!CaixaAberto)
                            {
                                MessageBox.Show("Seu app est� integrado com o SysMenu, Por favor abra o caixa antes de continuar", "Caixa Fechado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                this.Dispose();
                            }

                        }

                        bool TokenAutorizado = await Ifood.VerificaTokenValido();

                        if (TokenAutorizado)
                        {
                            Token.TokenDaSessao = tokenNoDb.accessToken;

                            FormMenuInicial menu = _formMenuInicial;
                            menu.Show();
                            this.Hide();
                        }

                    }
                    else if (!ConfigApp.IntegraIfood)
                    {
                        if (ConfigApp.IntegracaoSysMenu)
                        {
                            bool CaixaAberto = await ClsDeIntegracaoSys.VerificaCaixaAberto();

                            if (!CaixaAberto)
                            {
                                MessageBox.Show("Seu app est� integrado com o SysMenu, Por favor abra o caixa antes de continuar", "Caixa Fechado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                this.Dispose();
                            }

                        }

                        FormMenuInicial menu = _formMenuInicial;
                        menu.Show();
                        await EscondeForm1Async();
                        FormMenuInicial.panelPedidos.Invoke(new Action(async () => FormMenuInicial.MudaStatusMerchant()));
                    }


                }

            }
            catch (Exception ex)
            {
                await Logs.CriaLogDeErro(ex.ToString());
            }


        }

        public async Task EscondeForm1Async()
        {
            try
            {
                await Task.Delay(100);

                this.Hide();
            }
            catch (Exception ex)
            {
                Logs.CriaLogDeErro(ex.ToString());
            }
        }

        private async void pictureBoxCadeado_Click(object sender, EventArgs e)
        {
            try
            {
                panelInstrucoes.Visible = false;
                using (ApplicationDbContext db = await _context.GetContextoAsync())
                {
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
                        UserCodeReturnFromAPI.CodeVerifier = codesOfVerif.authorizationCodeVerifier; //seta o valor em uma propriedade static para podermos pegar no proximo m�todo 

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
            }
            catch (Exception ex)
            {
                await Logs.CriaLogDeErro(ex.ToString());
            }

        }

        private void pictureBoxInfo_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Caso nescessite de mais ajuda contatar a SysLogica", "Informa��es", MessageBoxButtons.OK);
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

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.notifyIcon1.Dispose();
        }

        private void notifyIcon1_BalloonTipClicked(object sender, EventArgs e)
        {
            this.notifyIcon1.Dispose();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            NewFormConfiguracoes newFormConfiguracoes = new NewFormConfiguracoes(new MeuContexto());
            newFormConfiguracoes.ShowDialog();
        }
    }



}
