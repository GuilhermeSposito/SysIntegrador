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
using System.Net.Http.Json;
using SysIntegradorApp.data;


namespace SysIntegradorApp.ClassesDeConexaoComApps;

internal class Ifood
{


    public static async Task Polling() //pulling feito de 30 em 30 Segundos, Caso seja encontrado algum novo pedido ele chama o GetPedidos
    {
        string url = @"https://merchant-api.ifood.com.br/order/v1.0/events";
        try
        {
            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token.TokenDaSessao);
            var reponse = await client.GetAsync($"{url}:polling");

            int statusCode = (int)reponse.StatusCode;
            if (statusCode == 200)
            {

                string jsonContent = await reponse.Content.ReadAsStringAsync();
                List<Polling>? pollings = JsonSerializer.Deserialize<List<Polling>>(jsonContent); //pedidos nesse caso é o pulling 

                foreach (var P in pollings)
                {
                    switch (P.code)
                    {
                        case "PLC": //caso entre aqui é porque é um novo pedido
                            await SetPedido(P.orderId, P);
                            await AvisarAcknowledge(P);
                            ConfirmarPedido(P);
                            break;
                        case "CFM":
                            await AtualizarStatusPedido(P);
                            await AvisarAcknowledge(P);
                            break;
                        case "CAN":
                            //mandaria um pedido pro ifood cancelando 
                            await AtualizarStatusPedido(P);
                            await AvisarAcknowledge(P);
                            break;
                        case "CON": //mudaria o status ou na tabela do sys menu
                            await AtualizarStatusPedido(P);
                            await AvisarAcknowledge(P);
                            break;
                        case "DSP":
                            await AtualizarStatusPedido(P);
                            await AvisarAcknowledge(P);
                            break;
                    }
                }

                FormMenuInicial.panelPedidos.Invoke(new Action(async () => FormMenuInicial.SetarPanelPedidos()));

            }

        }
        catch (Exception ex)
        {
           MessageBox.Show(ex.ToString(), "ERRO NO PULLING");
        }

    }


    //função para avisar para o ifood o ACK
    public static async Task AvisarAcknowledge(Polling polling)
    {
        string? url = @"https://merchant-api.ifood.com.br/order/v1.0/events";
        try
        {
            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token.TokenDaSessao);
            List<Polling> pollingList = new List<Polling>();
            pollingList.Add(polling);


            var polingToJson = JsonSerializer.Serialize(pollingList);
            StringContent content = new StringContent(polingToJson, Encoding.UTF8, "application/json");

            await client.PostAsync($"{url}/acknowledgment", content);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "OPS");
        }
    }


    public static async Task AtualizarStatusPedido(Polling P)
    {
        try
        {
            using ApplicationDbContext db = new ApplicationDbContext();

            bool verificaSeExistePedido = db.parametrosdopedido.Any(x => x.Id == P.orderId);

            if (verificaSeExistePedido)
            {
                await Console.Out.WriteLineAsync("\nStatus Do pedido atualizado com sucesso\n");
                var pedido = db.parametrosdopedido.Where(x => x.Id == P.orderId).FirstOrDefault();
                pedido.Situacao = P.fullCode;
                db.SaveChanges();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Erro ao atualizar Status Pedido", "OPS");
        }
    }


    //Função que Insere o pediddo que vem no pulling no banco de dados

    public static async Task SetPedido(string? orderId, Polling P)
    {
        string url = $"https://merchant-api.ifood.com.br/order/v1.0/orders/{P.orderId}";
        try
        {
            using var db = new ApplicationDbContext();
            bool verificaSeExistePedido = db.parametrosdopedido.Any(x => x.Id == P.orderId);

            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token.TokenDaSessao);
            HttpResponseMessage response = await client.GetAsync(url);

            string? jsonContent = await response.Content.ReadAsStringAsync();
            PedidoCompleto? pedidoCompletoDeserialiado = JsonSerializer.Deserialize<PedidoCompleto>(jsonContent);

            if (verificaSeExistePedido == false)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                await Console.Out.WriteLineAsync("\nPedido Adicionado no banco de dados\n");
                Console.ForegroundColor = ConsoleColor.White;

                string? mesa = pedidoCompletoDeserialiado.takeout.takeoutDateTime == null ? "WEB" : "WEBB";

                int insertNoSysMenuConta = await IntegracaoSequencia(mesa: mesa, cortesia: pedidoCompletoDeserialiado.total.benefits,
                     taxaEntrega: pedidoCompletoDeserialiado.total.deliveryFee,
                     taxaMotoboy: 0.00f, tipoPagamento: pedidoCompletoDeserialiado.payments.methods[0].method,
                     valorPagamento: pedidoCompletoDeserialiado.payments.methods[0].value,
                     dtInicio: pedidoCompletoDeserialiado.createdAt.Substring(0, 10),
                     hrInicio: pedidoCompletoDeserialiado.createdAt.Substring(11, 5),
                     contatoNome: pedidoCompletoDeserialiado.customer.name,
                     usuario: "CAIXA",
                     dataSaida: pedidoCompletoDeserialiado.createdAt.Substring(0, 10),
                     hrSaida: pedidoCompletoDeserialiado.createdAt.Substring(11, 5),
                     obsConta1: "teste1",
                     obsConta2: "Teste2",
                     endEntrega: pedidoCompletoDeserialiado.delivery.deliveryAddress.formattedAddress == null ? "RETIRADA" : pedidoCompletoDeserialiado.delivery.deliveryAddress.formattedAddress,
                     bairEntrega: pedidoCompletoDeserialiado.delivery.deliveryAddress.neighborhood == null ? "RETIRADA" : pedidoCompletoDeserialiado.delivery.deliveryAddress.neighborhood,
                     entregador: pedidoCompletoDeserialiado.delivery.deliveredBy == null ? "RETIRADA" : pedidoCompletoDeserialiado.delivery.deliveredBy
                     ); //fim dos parâmetros do método de integração

                db.parametrosdopedido.Add(new ParametrosDoPedido() { Id = P.orderId, Json = jsonContent, Situacao = P.fullCode, Conta = insertNoSysMenuConta });
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

    public static async Task<int> IntegracaoSequencia(string? mesa, //COMEÇO DOS PARÂMETROS DO MÉTODO
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
                    Console.ForegroundColor = ConsoleColor.Red;
                    await Console.Out.WriteLineAsync("\nPedido Inserido no banco de dados do access Também\n");
                    Console.ForegroundColor = ConsoleColor.White;
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

    public static async void ConfirmarPedido(Polling P)
    {
        string url = $"https://merchant-api.ifood.com.br/order/v1.0/orders/{P.orderId}/confirm";
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


}
