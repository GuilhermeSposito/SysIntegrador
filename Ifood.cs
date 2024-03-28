using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.EntityFrameworkCore;
using SysIntegradorApp.ClassesAuxiliares;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace SysIntegradorApp;

internal class Ifood
{
    private static System.Timers.Timer aTimer;

    public static async Task Pulling() //pulling feito de 30 em 30 Segundos, Caso seja encontrado algum novo pedido ele chama o GetPedidos
    {
        string url = @"https://merchant-api.ifood.com.br/order/v1.0/events";
        // Token.TokenDaSessao = "eyJraWQiOiJlZGI4NWY2Mi00ZWY5LTExZTktODY0Ny1kNjYzYmQ4NzNkOTMiLCJ0eXAiOiJKV1QiLCJhbGciOiJSUzUxMiJ9.eyJzdWIiOiJiZDg2MmYwNy0zYTgxLTRkZTYtYWM5Ni05NzJiNjZhNDljZTciLCJvd25lcl9uYW1lIjoiZ3VpbGhlcm1ldGVzdGVzIiwiaXNzIjoiaUZvb2QiLCJjbGllbnRfaWQiOiJjYzQ0Y2Q2MS1jYmI3LTQ0MjQtOTE5Yi1hM2RmNDI4N2FlYzEiLCJhcHBfbmFtZSI6Imd1aWxoZXJtZXRlc3Rlcy10ZXN0ZS1kIiwiYXVkIjpbInNoaXBwaW5nIiwiY2F0YWxvZyIsInJldmlldyIsImZpbmFuY2lhbCIsIm1lcmNoYW50IiwibG9naXN0aWNzIiwiZ3JvY2VyaWVzIiwiZXZlbnRzIiwib3JkZXIiLCJvYXV0aC1zZXJ2ZXIiXSwic2NvcGUiOlsic2hpcHBpbmciLCJjYXRhbG9nIiwicmV2aWV3IiwibWVyY2hhbnQiLCJsb2dpc3RpY3MiLCJncm9jZXJpZXMiLCJldmVudHMiLCJvcmRlciIsImNvbmNpbGlhdG9yIl0sInR2ZXIiOiJ2MiIsIm1lcmNoYW50X3Njb3BlIjpbIjkzNjIwMThhLTZhZTItNDM5Yy05NjhiLWE0MDE3N2EwODVlYTptZXJjaGFudCIsIjkzNjIwMThhLTZhZTItNDM5Yy05NjhiLWE0MDE3N2EwODVlYTpvcmRlciIsIjkzNjIwMThhLTZhZTItNDM5Yy05NjhiLWE0MDE3N2EwODVlYTpjYXRhbG9nIiwiOTM2MjAxOGEtNmFlMi00MzljLTk2OGItYTQwMTc3YTA4NWVhOmNvbmNpbGlhdG9yIiwiOTM2MjAxOGEtNmFlMi00MzljLTk2OGItYTQwMTc3YTA4NWVhOnJldmlldyIsIjkzNjIwMThhLTZhZTItNDM5Yy05NjhiLWE0MDE3N2EwODVlYTpsb2dpc3RpY3MiLCI5MzYyMDE4YS02YWUyLTQzOWMtOTY4Yi1hNDAxNzdhMDg1ZWE6c2hpcHBpbmciLCI5MzYyMDE4YS02YWUyLTQzOWMtOTY4Yi1hNDAxNzdhMDg1ZWE6Z3JvY2VyaWVzIiwiOTM2MjAxOGEtNmFlMi00MzljLTk2OGItYTQwMTc3YTA4NWVhOmV2ZW50cyJdLCJleHAiOjE3MTEwNzAzNzEsImlhdCI6MTcxMTA0ODc3MSwianRpIjoiYmQ4NjJmMDctM2E4MS00ZGU2LWFjOTYtOTcyYjY2YTQ5Y2U3OmNjNDRjZDYxLWNiYjctNDQyNC05MTliLWEzZGY0Mjg3YWVjMSIsIm1lcmNoYW50X3Njb3BlZCI6dHJ1ZX0.K6i31FGzFFaJmc2nxCN5u3s9pNGaBr_SfsAkQpBj_zY4Ve7BQ_oPX-j5p80rszThN0fw-VPm-teQFwN5T0E7X3itab2cOklALgxHjy0Um5DP0xcV1IW4ywj6E49rtlkAVBUe9KNa2AiVe-zV2gaMZE7x9N9PzBzQJiqZyeaF50I";
        try
        {
            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token.TokenDaSessao);
            HttpResponseMessage reponse = await client.GetAsync($"{url}:polling");

            int statusCode = (int)reponse.StatusCode;
            if (statusCode == 200)
            {

                string jsonContent = await reponse.Content.ReadAsStringAsync();
                List<Pedido>? pedidos = JsonSerializer.Deserialize<List<Pedido>>(jsonContent);



                using (var dbContex = new ApplicationDbContext())
                {
                    var pullingsNaBase = dbContex.pulling.ToList();
                    foreach (var pullingAtual in pedidos)
                    {
                        var confereSeJaExiste = pullingsNaBase.Any((p) => p.id.Contains(pullingAtual.id));

                        await Console.Out.WriteLineAsync(pullingAtual.fullCode);

                        if (!confereSeJaExiste) //só entra aqui caso o pulling não existir
                        {
                            dbContex.pulling.Add(new Pulling() { id = pullingAtual.id });
                            dbContex.SaveChanges();
                            await SetPedido(pullingAtual.orderId, pullingAtual.fullCode);
                        }

                        var order = dbContex.parametrosdopedido.Where(p => p.Id == pullingAtual.orderId).FirstOrDefault();

                        if (order != null)
                        {
                            order.Situacao = pullingAtual.fullCode;
                            dbContex.SaveChanges();
                        }

                    }

                    var pulingsToJson = JsonSerializer.Serialize(pullingsNaBase);
                    StringContent content = new StringContent(pulingsToJson, Encoding.UTF8, "application/json");

                    await client.PostAsync($"{url}/acknowledgment", content);

                    FormMenuInicial.panelPedidos.Invoke(new Action(async () => FormMenuInicial.SetarPanelPedidos()));

                }

            }

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message, ex.StackTrace);
        }

    }

    //Função que Insere o pediddo que vem no pulling no banco de dados
    public static async Task SetPedido(string orderId, string statusCode = "PLACED")
    {
        // Token.TokenDaSessao = "eyJraWQiOiJlZGI4NWY2Mi00ZWY5LTExZTktODY0Ny1kNjYzYmQ4NzNkOTMiLCJ0eXAiOiJKV1QiLCJhbGciOiJSUzUxMiJ9.eyJzdWIiOiJiZDg2MmYwNy0zYTgxLTRkZTYtYWM5Ni05NzJiNjZhNDljZTciLCJvd25lcl9uYW1lIjoiZ3VpbGhlcm1ldGVzdGVzIiwiaXNzIjoiaUZvb2QiLCJjbGllbnRfaWQiOiJjYzQ0Y2Q2MS1jYmI3LTQ0MjQtOTE5Yi1hM2RmNDI4N2FlYzEiLCJhcHBfbmFtZSI6Imd1aWxoZXJtZXRlc3Rlcy10ZXN0ZS1kIiwiYXVkIjpbInNoaXBwaW5nIiwiY2F0YWxvZyIsInJldmlldyIsImZpbmFuY2lhbCIsIm1lcmNoYW50IiwibG9naXN0aWNzIiwiZ3JvY2VyaWVzIiwiZXZlbnRzIiwib3JkZXIiLCJvYXV0aC1zZXJ2ZXIiXSwic2NvcGUiOlsic2hpcHBpbmciLCJjYXRhbG9nIiwicmV2aWV3IiwibWVyY2hhbnQiLCJsb2dpc3RpY3MiLCJncm9jZXJpZXMiLCJldmVudHMiLCJvcmRlciIsImNvbmNpbGlhdG9yIl0sInR2ZXIiOiJ2MiIsIm1lcmNoYW50X3Njb3BlIjpbIjkzNjIwMThhLTZhZTItNDM5Yy05NjhiLWE0MDE3N2EwODVlYTptZXJjaGFudCIsIjkzNjIwMThhLTZhZTItNDM5Yy05NjhiLWE0MDE3N2EwODVlYTpvcmRlciIsIjkzNjIwMThhLTZhZTItNDM5Yy05NjhiLWE0MDE3N2EwODVlYTpjYXRhbG9nIiwiOTM2MjAxOGEtNmFlMi00MzljLTk2OGItYTQwMTc3YTA4NWVhOmNvbmNpbGlhdG9yIiwiOTM2MjAxOGEtNmFlMi00MzljLTk2OGItYTQwMTc3YTA4NWVhOnJldmlldyIsIjkzNjIwMThhLTZhZTItNDM5Yy05NjhiLWE0MDE3N2EwODVlYTpsb2dpc3RpY3MiLCI5MzYyMDE4YS02YWUyLTQzOWMtOTY4Yi1hNDAxNzdhMDg1ZWE6c2hpcHBpbmciLCI5MzYyMDE4YS02YWUyLTQzOWMtOTY4Yi1hNDAxNzdhMDg1ZWE6Z3JvY2VyaWVzIiwiOTM2MjAxOGEtNmFlMi00MzljLTk2OGItYTQwMTc3YTA4NWVhOmV2ZW50cyJdLCJleHAiOjE3MTA5NTk3NDIsImlhdCI6MTcxMDkzODE0MiwianRpIjoiYmQ4NjJmMDctM2E4MS00ZGU2LWFjOTYtOTcyYjY2YTQ5Y2U3OmNjNDRjZDYxLWNiYjctNDQyNC05MTliLWEzZGY0Mjg3YWVjMSIsIm1lcmNoYW50X3Njb3BlZCI6dHJ1ZX0.NO1_-hgj4h4XeN8bZXIWBKztACHnJZzWYnuvDClluXzjYE6b7sm7wwzbMow7wOHRHgkGjRkHduiUVNAFB7-yULunwX350PLRiIGxuBf_cFUyK1_xvO_M14p59s4yGkntobm6pj57ZH1MxPnbxTw4Rgftqc7eQCW54cfhdebbO7s";
        string url = $"https://merchant-api.ifood.com.br/order/v1.0/orders/{orderId}";
        try
        {
            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token.TokenDaSessao);
            HttpResponseMessage response = await client.GetAsync(url);

            if (Convert.ToInt32(response.StatusCode) == 404)
            {
                throw new HttpRequestException("Pedido Não Encontrado");
            }


            string? pedidoJson = await response.Content.ReadAsStringAsync();
            PedidoCompleto? pedidoCompletoTotal = JsonSerializer.Deserialize<PedidoCompleto>(pedidoJson);

            //pedidocompleto? pedidocompletoDB = JsonSerializer.Deserialize<pedidocompleto>(leituraDoPedido);

            //setar o id_pedido de cada objeto relacionado para inserção no banco
            //fazer o insert no banco de dados separando todo o pedido em tabelas relacionadas

            using (var db = new ApplicationDbContext())
            {
                ParametrosDoPedido pedidoDB = new ParametrosDoPedido(id: pedidoCompletoTotal.id, json: pedidoJson, situacao: statusCode) ;

                //inserir na tabela parâmetros do pedido
                db.parametrosdopedido.Add(pedidoDB);
                db.SaveChanges();
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(ex.Message);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }

    //função que está mostrando os pedidos no panel
    public static async Task<List<PedidoCompleto>> GetPedido(/*string pedido_id*/)
    {

        string path = @"C:\Users\gui-c\OneDrive\Área de Trabalho\primeiro\testeSeriliazeJson.json";
         List<PedidoCompleto> pedidos = new List<PedidoCompleto>();
        try
        {
            using ApplicationDbContext db = new ApplicationDbContext();

            List<ParametrosDoPedido> pedidosFromDb = db.parametrosdopedido.ToList();
            //adicionar cada json em uma lista para poder deserializar nas funções
            foreach(ParametrosDoPedido item in pedidosFromDb)
            {
                PedidoCompleto pedido = JsonSerializer.Deserialize<PedidoCompleto>(item.Json);
                pedidos.Add(pedido);
            }

            return pedidos;  
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
            await Console.Out.WriteLineAsync(ex.Message);
        }

        return pedidos;
    }

    public static async void SetTimer()//set timer pra fazer o acionamento a cada 30 segundos do pulling
    {

        aTimer = new System.Timers.Timer(30000);
        aTimer.Elapsed += OnTimedEvent;
        aTimer.AutoReset = true;
        aTimer.Enabled = true;

        await Console.Out.WriteLineAsync();
    }

    private static async void OnTimedEvent(System.Object source, ElapsedEventArgs e)
    {
        await Pulling();
        // Função para corrigir a diferença de thread 

    }
}
