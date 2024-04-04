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
using System.Data;
using System.Data.OleDb;


namespace SysIntegradorApp;

internal class Ifood
{
    private static System.Timers.Timer aTimer;

    public static async Task Pulling() //pulling feito de 30 em 30 Segundos, Caso seja encontrado algum novo pedido ele chama o GetPedidos
    {
        string url = @"https://merchant-api.ifood.com.br/order/v1.0/events";
        try
        {
            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token.TokenDaSessao);
            HttpResponseMessage reponse = await client.GetAsync($"{url}:polling");

            int statusCode = (int)reponse.StatusCode;
            if (statusCode == 200)
            {

                string jsonContent = await reponse.Content.ReadAsStringAsync();
                List<Pedido>? pedidos = JsonSerializer.Deserialize<List<Pedido>>(jsonContent); //pedidos nesse caso é o pulling 

                using (var dbContex = new ApplicationDbContext())
                {
                    var pullingsNaBase = dbContex.pulling.ToList();

                    foreach (var pullingAtual in pedidos)
                    {
                        var confereSeJaExiste = pullingsNaBase.Any((p) => p.id.Contains(pullingAtual.id));

                        if (!confereSeJaExiste) //só entra aqui caso o pulling não existir
                        {
                            dbContex.pulling.Add(new Pulling() { id = pullingAtual.id });
                        }

                        var confereSeJaExisteOPedido = dbContex.parametrosdopedido.Any((p) => p.Id.Contains(pullingAtual.orderId));

                        if (confereSeJaExisteOPedido)
                        {
                            var order = dbContex.parametrosdopedido.Where(p => p.Id == pullingAtual.orderId).FirstOrDefault();
                            order.Situacao = pullingAtual.fullCode;
                            dbContex.SaveChanges();
                        }

                        if (!confereSeJaExisteOPedido)
                        {
                            await SetPedido(pullingAtual.orderId, pullingAtual.fullCode);
                            ConfirmarPedido(pullingAtual.orderId);
                        }

                    }

                    var pullingsNaBaseAtualizado = dbContex.pulling.ToList();

                    var pulingsToJson = JsonSerializer.Serialize(pullingsNaBaseAtualizado);
                    StringContent content = new StringContent(pulingsToJson, Encoding.UTF8, "application/json");

                    await client.PostAsync($"{url}/acknowledgment", content);
                }

            }

            FormMenuInicial.panelPedidos.Invoke(new Action(async () => FormMenuInicial.SetarPanelPedidos()));

        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString(), "ERRO NO PULLING");
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


            using (var db = new ApplicationDbContext())
            {
                string? mesa = pedidoCompletoTotal.takeout.takeoutDateTime == null ? "WEB" : "WEBB";

                int insertNoSysMenuConta = IntegracaoSequencia(mesa: mesa, cortesia: pedidoCompletoTotal.total.benefits,
                    taxaEntrega: pedidoCompletoTotal.total.deliveryFee,
                    taxaMotoboy: 0.00f, tipoPagamento: pedidoCompletoTotal.payments.methods[0].method,
                    valorPagamento: pedidoCompletoTotal.payments.methods[0].value,
                    dtInicio: pedidoCompletoTotal.createdAt.Substring(0, 10),
                    hrInicio: pedidoCompletoTotal.createdAt.Substring(11, 5),
                    contatoNome: pedidoCompletoTotal.customer.name,
                    usuario: "CAIXA",
                    dataSaida: pedidoCompletoTotal.createdAt.Substring(0, 10),
                    hrSaida: pedidoCompletoTotal.createdAt.Substring(11, 5),
                    obsConta1: "teste1",
                    obsConta2: "Teste2",
                    endEntrega: pedidoCompletoTotal.delivery.deliveryAddress.formattedAddress == null ? "RETIRADA" : pedidoCompletoTotal.delivery.deliveryAddress.formattedAddress,
                    bairEntrega: pedidoCompletoTotal.delivery.deliveryAddress.neighborhood == null ? "RETIRADA" : pedidoCompletoTotal.delivery.deliveryAddress.neighborhood,
                    entregador: pedidoCompletoTotal.delivery.deliveredBy == null ? "RETIRADA" : pedidoCompletoTotal.delivery.deliveredBy
                    ); //fim dos parâmetros do método de integração


                ParametrosDoPedido pedidoDB = new ParametrosDoPedido(id: pedidoCompletoTotal.id, json: pedidoJson, situacao: statusCode, conta: insertNoSysMenuConta);

                //inserir na tabela parâmetros do pedido
                db.parametrosdopedido.Add(pedidoDB);
                db.SaveChanges();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString(), "ERRO AO INSERIR PEDIDO NO POSTGRES");
        }

    }

    //função que está retornando os pedidos para setar os pedidos no panel
    public static async Task<List<ParametrosDoPedido>> GetPedido(string? pedido_id = null)
    {
        List<ParametrosDoPedido> pedidosFromDb = new List<ParametrosDoPedido>();

        string path = @"C:\Users\gui-c\OneDrive\Área de Trabalho\primeiro\testeSeriliazeJson.json";
        List<PedidoCompleto> pedidos = new List<PedidoCompleto>();
        try
        {
            if (pedido_id != null)
            {
                using ApplicationDbContext dataBase = new ApplicationDbContext();

                pedidosFromDb = dataBase.parametrosdopedido.Where(p => p.Id == pedido_id).ToList();
                //adicionar cada json em uma lista para poder deserializar nas funções

                return pedidosFromDb;
            }

            using ApplicationDbContext db = new ApplicationDbContext();

            pedidosFromDb = db.parametrosdopedido.ToList();
            //adicionar cada json em uma lista para poder deserializar nas funções

            return pedidosFromDb;

        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "ERRO AO GETPEDIDO");
        }

        return pedidosFromDb;
    }

    public static int IntegracaoSequencia(string? mesa, //COMEÇO DOS PARÂMETROS DO MÉTODO
        float cortesia,
        float taxaEntrega,
        float taxaMotoboy,
        string? tipoPagamento,
        float valorPagamento,
        string? dtInicio,
        string? hrInicio,
        string? contatoNome,
        string? usuario,
        string? dataSaida,
        string? hrSaida,
        string? obsConta1,
        string? obsConta2 = null,
        string? endEntrega = "RETIRADA",
        string? bairEntrega = "RETIRADA",
        string? entregador = "RETIRADA"
        ) //método que está sendo usado para integrar a tabela contas do banco de dados com a tabela de pedido do SysIntegrador
    {

        string? pagamento = "PAGCRT";


        switch (tipoPagamento)
        {
            case "CREDIT":
                pagamento = "PAGCRT";
                break;
            case "MEAL_VOUCHER":
                pagamento = "VOUCHER";
                break;
            case "DEBIT":
                pagamento = "PAGCRT";
                break;
            case "PIX":
                pagamento = "PAGCRT";
                break;
            case "CASH":
                pagamento = "PAGDNH";
                break;
            case "BANK_PAY ":
                pagamento = "PAGCRT";
                break;
            case "FOOD_VOUCHER ":
                pagamento = "PAGCRT";
                break;
            default:
                pagamento = "PAGCRT";
                break;
        }

        int ultimoNumeroConta = 0;
        try
        {
            string banco = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\gui-c\OneDrive\Área de Trabalho\SysIntegrador\CONTAS.mdb";
            List<int> numerosSequencia = new List<int>();
            numerosSequencia.Clear();

            using (OleDbConnection connection = new OleDbConnection(banco))
            {
                connection.Open();

                string sqlInsert = $"INSERT INTO Sequencia (MESA, STATUS,CORTESIA ,TAXAENTREGA,TAXAMOTOBOY, {pagamento}, DTINICIO, HRINICIO, ENDENTREGA, BAIENTREGA, CONTATO, ENTREGADOR, USUARIO, DTSAIDA, HRSAIDA, OBSCONTA1, OBSCONTA2) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";

                using (OleDbCommand command = new OleDbCommand(sqlInsert, connection))
                {
                    // Parâmetros para a consulta SQL
                    command.Parameters.AddWithValue("@MESA", mesa);
                    command.Parameters.AddWithValue("@STATUS", "P");
                    command.Parameters.AddWithValue("@CORTESIA", cortesia);
                    command.Parameters.AddWithValue("@TAXAENTREGA", taxaEntrega);
                    command.Parameters.AddWithValue("@TAXAMOTOBOY", taxaMotoboy);
                    command.Parameters.AddWithValue($"@{pagamento}", valorPagamento);
                    command.Parameters.AddWithValue("@DTINICIO", dtInicio.Replace("-", "/"));
                    command.Parameters.AddWithValue("@HRINICIO", hrInicio);
                    command.Parameters.AddWithValue("@ENDENTREGA", endEntrega); //se vier WEBB aqui vai ser null
                    command.Parameters.AddWithValue("@BAIENTREGA", bairEntrega);//se vier WEBB aqui vai ser null
                    command.Parameters.AddWithValue("@CONTATO", contatoNome);
                    command.Parameters.AddWithValue("@ENTREGADOR", entregador); //se vier WEBB aqui vai ser null
                    command.Parameters.AddWithValue("@USUARIO", usuario);
                    command.Parameters.AddWithValue("@DTSAIDA", dataSaida.Replace("-", "/"));
                    command.Parameters.AddWithValue("@HRSAIDA", hrSaida);
                    command.Parameters.AddWithValue("@OBSCONTA1", obsConta1);
                    command.Parameters.AddWithValue("@OBSCONTA2", obsConta2);


                    // Executa o comando SQL
                    int rowsAffected = command.ExecuteNonQuery();


                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("Inserção realizada com sucesso!");

                        //Se a inserção foi feita com sucesso, nós vamos pegar o número da sequencia
                        string sqlQuery = "SELECT * FROM Sequencia";

                        using (OleDbCommand comando = new OleDbCommand(sqlQuery, connection))
                        using (OleDbDataReader reader = comando.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine("Pedidos:\n");
                                while (reader.Read())
                                {
                                    // Exibe o conteúdo de cada coluna na linha
                                    for (int i = 0; i < reader.FieldCount; i++)
                                    {
                                        if (reader.GetName(i) == "CONTA")
                                        {
                                            //se entrar aqui vai adicionar nos número da sequencia em um array 
                                            int valorConvertido = Convert.ToInt32(reader.GetValue(i));
                                            numerosSequencia.Add(valorConvertido);
                                        }
                                    }
                                }

                                //aqui retorna o ultimo número inserido
                                ultimoNumeroConta = numerosSequencia[numerosSequencia.Count() - 1];

                                return ultimoNumeroConta;
                            } // fechamento if hasRows
                        }   //fechamento terceiro using

                    } //fechamento de chave do if RowsAfected
                }//fechamento chave segundo using 
            }//fechamento chave primeiro using
        }// fechamento chave if
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString(), "ERRO AO INSERIR O PEDIDO NO BANCO DE DADOS DO ACCESS");
        }

        return ultimoNumeroConta;
    }


    public static async void ConfirmarPedido(string idPedido)
    {
        string url = $"https://merchant-api.ifood.com.br/order/v1.0/orders/{idPedido}/confirm";
        try
        {
            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token.TokenDaSessao);
            StringContent content = new StringContent("", Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(url, content);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "ERRO AO CONFIRMAR PEDIDO");
        }
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
        try
        {
            await Pulling();
            // Função para corrigir a diferença de thread 
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "ERRO AO ATIVAR PULLING");
        }
     

    }
}
