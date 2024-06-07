using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysIntegradorApp.ClassesAuxiliares.Verificacoes;

public class VerificaInternet
{
    private static readonly HttpClient client = new HttpClient();

    public static async Task<bool> InternetAtiva()
    {
        try
        {
            HttpResponseMessage response = await client.GetAsync("http://www.google.com");
            response.EnsureSuccessStatusCode();
            return true;
        }
        catch (HttpRequestException)
        {
            return false;
        }
    }
}
