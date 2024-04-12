using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysIntegradorApp.ClassesDeConexaoComApps;

public class DelMatch
{
    public static async Task GerarPedido()
    {
        string? url = "http://100.26.63.137/api/deliveries/default/";
        try
        {
            using HttpClient client = new HttpClient();
            string jsonContent = @"{""id"":""dd56e3a213da0d221091d3bc6a0e621071550b80"",""shortReference"":6555,""createdAt"":"""",""type"":""DELIVERY"",""time_max"":"""",""merchant"":{""restaurantId"":""ca04c7d795a171571f4e5e301cea118a3ef282d0"",""name"":""Pastéis e Panquecas"",""id"":""62e91b20e390370012f98023"",""unit"":""62e91b20e390370012f9802e""},""customer"":{""name"":""Julia Camargo Cavalli"",""phone"":""08007053040"",""taxPayerIdentificationNumber"":""52518691855""},""deliveryAddress"":{""formattedAddress"":""Av. Bruno Ruggiero Filho, Nº 101"",""country"":""BR"",""state"": ""SP"",""city"":""São Carlos"",""coordinates"":{""latitude"":-22.010634, ""longitude"":-47.914743 },""neighborhood"":""Parque Santa Felicia Jardim"",""streetName"":""Av. Bruno Ruggiero Filho"",""streetNumber"":""101"",""postalCode"":""13562420"",""complement"":""bloco B número 79""}}";
            StringContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(url, content);
            string resposta = await response.Content.ReadAsStringAsync(); ;


           MessageBox.Show($"Leitura do endpoint = {resposta}\n" +
                $"Status Code da Requisição HTTP: {(int)response.StatusCode}", "Retorno");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
