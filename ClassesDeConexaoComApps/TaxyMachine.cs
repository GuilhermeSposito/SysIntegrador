using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.Logging;
using SysIntegradorApp.ClassesAuxiliares;
using SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoCCM;
using SysIntegradorApp.ClassesAuxiliares.logs;
using SysIntegradorApp.data;
using SysIntegradorApp.data.InterfaceDeContexto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace SysIntegradorApp.ClassesDeConexaoComApps;

public class TaxyMachine
{
    private readonly IMeuContexto _Context;
    public string? _ApiKey { get; set; }
    public string? UrlApi { get { return "https://cloud.taximachine.com.br/api/integracao/"; } }
    public string? Username { get; set; } = string.Empty;
    public string? Password { get; set; } = string.Empty;

    public TaxyMachine(MeuContexto context)
    {
        _Context = context;
    }

    public async Task<string?> GetApiAuthAsync()
    {
        try
        {
            await using (ApplicationDbContext db = await _Context.GetContextoAsync())
            {
                ParametrosDoSistema? ConfigSist = await db.parametrosdosistema.FirstOrDefaultAsync();

                _ApiKey = ConfigSist.ApiKeyTaxyMachine; 
                Username = ConfigSist.UserNameTaxyMachine;
                Password = ConfigSist.PasswordTaxyMachine;

                return ConfigSist.ApiKeyTaxyMachine;
            }
        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.Message);
            MessageBox.Show("Erro ao ter chave de acesso do banco de dados", "Ops", MessageBoxButtons.OK, MessageBoxIcon.Error) ;
        }
        return null;
    }

    public async Task GetInfosEmpresa()
    {
        try
        {
            HttpResponseMessage? Response = await ReqConstructor("GET", "empresa");

            if (Response is null)
                throw new Exception("Erro ao enviar requisição Taxy Machine");


            MessageBox.Show(await Response.Content.ReadAsStringAsync());

        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.Message);
            MessageBox.Show("Erro ao acessar informações da empresa!", "Ops", MessageBoxButtons.OK, MessageBoxIcon.Error);

        }
    }

    public async Task<HttpResponseMessage?> ReqConstructor(string? metodo, string endpoint, string? content = null)
    {
        try
        {
            string? url = UrlApi + endpoint;

            string? ApiKey = await GetApiAuthAsync();

            if (metodo == "GET")
            {
                using var requestClient = new HttpClient();
                requestClient.DefaultRequestHeaders.Add("api-key", ApiKey);

                var byteArray = Encoding.ASCII.GetBytes($"{Username}:{Password}");
                requestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                return await requestClient.GetAsync(url);
            }

        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.Message);
        }
        return null;
    }
}
