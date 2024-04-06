using System.Net.Http.Headers;
using System.Security.Policy;
using System.Text.Json;
using ExCSS;
using Microsoft.EntityFrameworkCore;
using SysIntegradorApp.ClassesAuxiliares;
using SysIntegradorApp.ClassesAuxiliares;
using System.Diagnostics;
using System.Windows.Forms;
using SysIntegradorApp.data;


namespace SysIntegradorApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void LabelInfoToUser_Click(object sender, EventArgs e)
        {

        }

        private async void PedirCod_Click(object sender, EventArgs e)
        {
            try
            {
                AvisoParaPegarCod.Visible = false;

                string url = "https://merchant-api.ifood.com.br/authentication/v1.0/oauth/";

                using (HttpClient client = new HttpClient())
                {
                    FormUrlEncodedContent formData = new FormUrlEncodedContent(new[]
                        {
                        new KeyValuePair<string, string>("clientId", UserCodes.clientId)
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
                    LabelInfoToUser.Visible = true;
                    LabelCodeToUser.Visible = true;
                    LabelCodeToUser.Text = codesOfVerif.userCode;
                    CodeLabel1.Visible = true;
                    CodeFromUser.Visible = true;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ops", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private async void BrnAutorizar_Click(object sender, EventArgs e)
        {
            string url = "https://merchant-api.ifood.com.br/authentication/v1.0/oauth/";
            try
            {
                string? codeFromMenu = CodeFromUser.Text;

                FormUrlEncodedContent formDataToGetTheToken = new FormUrlEncodedContent(new[]
                     {
                        new KeyValuePair<string, string>("grantType", "authorization_code"),
                        new KeyValuePair<string, string>("clientId", UserCodes.clientId),
                        new KeyValuePair<string, string>("clientSecret", UserCodes.clientSecret),
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

                using ApplicationDbContext db = new ApplicationDbContext();

                //var order = dbContex.parametrosdopedido.Where(p => p.Id == pullingAtual.orderId).FirstOrDefault();

                var tokenDB = db.parametrosdeautenticacao.Where(p => p.id == 1).FirstOrDefault();

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

        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                string idMerchant = "9362018a-6ae2-439c-968b-a40177a085ea";
                string url = $"https://merchant-api.ifood.com.br/merchant/v1.0/merchants/{idMerchant}/status";

                using (ApplicationDbContext db = new ApplicationDbContext())
                {

                    Token? tokenNoDb = db.parametrosdeautenticacao.ToList().FirstOrDefault();


                    using HttpClient client = new HttpClient();
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenNoDb.accessToken);
                    HttpResponseMessage response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        Token.TokenDaSessao = tokenNoDb.accessToken;

                        FormMenuInicial menu = new FormMenuInicial();
                        menu.Show();
                        this.Hide();
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Ops");
            }


        }
    }
}
