using Entidades;
using Newtonsoft.Json;
using SysIntegradorApp.ClassesAuxiliares;
using SysIntegradorApp.data;
using SysIntegradorApp.Forms;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace SysIntegradorApp.ClassesDeConexaoComApps;

public class DelMatch : Ifood
{

    public static List<Sequencia> ListarPedidosAbertos() //método que serve para enviarmos um pedido
    {
        List<Sequencia> sequencias = new List<Sequencia>();
        try
        {
            using ApplicationDbContext dbPostgres = new ApplicationDbContext();
            ParametrosDoSistema? opcSistema = dbPostgres.parametrosdosistema.ToList().FirstOrDefault();

            string? caminhoBancoAccess = opcSistema.CaminhodoBanco;

            string entregador = "99";
            string SqlSelectIntoCadastros = $"SELECT * FROM Sequencia WHERE TRIM(ENTREGADOR) = @ENTREGADOR AND DelMatchId IS NULL";

            using (OleDbConnection connection = new OleDbConnection(caminhoBancoAccess))
            {
                connection.Open();


                using (OleDbCommand selectCommand = new OleDbCommand(SqlSelectIntoCadastros, connection))
                {
                    selectCommand.Parameters.AddWithValue("@ENTREGADOR", entregador);

                    // Executar a consulta SELECT
                    using (OleDbDataReader reader = selectCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string telefone = reader["TELEFONE"].ToString();
                            Sequencia sequencia = PesquisaClientesNoCadastro(telefone.Trim());

                            if (sequencia.DeliveryAddress.FormattedAddress != null)
                            {
                                sequencia.numConta = Convert.ToInt32(reader["CONTA"].ToString());
                                sequencia.ValorConta = Convert.ToDecimal(reader["ACRESCIMO"]) +
                                                       Convert.ToDecimal(reader["SERVICO"]) +
                                                       Convert.ToDecimal(reader["COUVERT"]) +
                                                       Convert.ToDecimal(reader["TAXAENTREGA"]) +
                                                       Convert.ToDecimal(reader["TAXAMOTOBOY"]) +
                                                       Convert.ToDecimal(reader["PAGDNH"]) +
                                                       Convert.ToDecimal(reader["PAGCHQ"]) +
                                                       Convert.ToDecimal(reader["PAGCRT"]) +
                                                       Convert.ToDecimal(reader["PAGTKT"]) +
                                                       Convert.ToDecimal(reader["PAGCVALE"]) +
                                                       Convert.ToDecimal(reader["PAGCC"]) +
                                                       Convert.ToDecimal(reader["PAGONLINE"]) +
                                                       Convert.ToDecimal(reader["VOUCHER"]) -
                                                       Convert.ToDecimal(reader["TROCO"]) -
                                                       Convert.ToDecimal(reader["CORTESIA"]);
                                string NumConta = sequencia.numConta.ToString();
                                
                                Guid idPedido = Guid.NewGuid();

                                sequencia.DelMatchId = idPedido.ToString();
                                sequencia.Id = "dd56e3a213da0d221091d3bc6a0e621071550b80";
                                sequencia.ShortReference = idPedido.ToString();
                                sequencia.CreatedAt = "";
                                sequencia.Type = "DELIVERY";
                                sequencia.TimeMax = "";

                                sequencia.Merchant.RestaurantId = opcSistema.DelMatchId;
                                sequencia.Merchant.Name = opcSistema.NomeFantasia;
                                sequencia.Merchant.Id = opcSistema.DelMatchId;
                                sequencia.Merchant.Unit = "62e91b20e390370012f9802e";

                                sequencias.Add(sequencia);
                            }

                        }


                    }

                }
                return sequencias;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString(), "Erro Ao listar Pedidos Abertos");
        }

        return sequencias;
    }

    public static List<Sequencia> ListarPedidosJaEnviados()
    {
        List<Sequencia> sequencias = new List<Sequencia>();
        try
        {
            using ApplicationDbContext dbPostgres = new ApplicationDbContext();
            ParametrosDoSistema? opcSistema = dbPostgres.parametrosdosistema.ToList().FirstOrDefault();

            string? caminhoBancoAccess = opcSistema.CaminhodoBanco;

            string entregador = "99";
            string SqlSelectIntoCadastros = $"SELECT * FROM Sequencia WHERE TRIM(ENTREGADOR) = @ENTREGADOR AND DelMatchId IS NOT NULL";

            using (OleDbConnection connection = new OleDbConnection(caminhoBancoAccess))
            {
                connection.Open();


                using (OleDbCommand selectCommand = new OleDbCommand(SqlSelectIntoCadastros, connection))
                {
                    selectCommand.Parameters.AddWithValue("@ENTREGADOR", entregador);

                    // Executar a consulta SELECT
                    using (OleDbDataReader reader = selectCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Sequencia sequencia = PesquisaClientesNoCadastro(reader["TELEFONE"].ToString());

                            if (sequencia.DeliveryAddress.FormattedAddress != null)
                            {

                                sequencia.numConta = Convert.ToInt32(reader["CONTA"].ToString());
                                sequencia.ValorConta = Convert.ToDecimal(reader["ACRESCIMO"]) +
                                                       Convert.ToDecimal(reader["SERVICO"]) +
                                                       Convert.ToDecimal(reader["COUVERT"]) +
                                                       Convert.ToDecimal(reader["TAXAENTREGA"]) +
                                                       Convert.ToDecimal(reader["TAXAMOTOBOY"]) +
                                                       Convert.ToDecimal(reader["PAGDNH"]) +
                                                       Convert.ToDecimal(reader["PAGCHQ"]) +
                                                       Convert.ToDecimal(reader["PAGCRT"]) +
                                                       Convert.ToDecimal(reader["PAGTKT"]) +
                                                       Convert.ToDecimal(reader["PAGCVALE"]) +
                                                       Convert.ToDecimal(reader["PAGCC"]) +
                                                       Convert.ToDecimal(reader["PAGONLINE"]) +
                                                       Convert.ToDecimal(reader["VOUCHER"]) -
                                                       Convert.ToDecimal(reader["TROCO"]) -
                                                       Convert.ToDecimal(reader["CORTESIA"]);
                                string NumConta = sequencia.numConta.ToString();

                                sequencia.ShortReference = NumConta.PadLeft(4, '0');
                                sequencia.DelMatchId = reader["DelMatchId"].ToString();

                                sequencias.Add(sequencia);
                            }
                        }

                    }

                }
                return sequencias;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString(), "Erro Ao listar Pedidos Abertos");
        }

        return sequencias;
    }

    public static async Task GerarPedido(string? jsonContent)
    {
        string? url = "https://delmatchapp.com/api/deliveries/default/";
        try
        {
            using HttpClient client = new HttpClient();
            StringContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(url, content);
            string resposta = await response.Content.ReadAsStringAsync(); 
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public static async Task<ClsDeserializacaoDelMatchEntrega> GetPedido(string? delMatchId)
    {
        ClsDeserializacaoDelMatchEntrega pedido = new ClsDeserializacaoDelMatchEntrega();

        string apiUrl = "https://delmatchapp.com/api/deliveries-list/";
        try
        {
            using ApplicationDbContext db = new ApplicationDbContext();
            var ConfigsSistema = db.parametrosdosistema.ToList().FirstOrDefault();

            string token = ConfigsSistema.DelMatchId;

            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Token " + token);

            var response = await client.GetAsync(apiUrl);

            string responseString = await response.Content.ReadAsStringAsync();

            string responseJson = await response.Content.ReadAsStringAsync();
            var pedidos = JsonConvert.DeserializeObject<List<ClsDeserializacaoDelMatchEntrega>>(responseJson);

            pedido = pedidos.Where(x => x.IdOrder == delMatchId).FirstOrDefault();

            if (pedido == null)
            {
                return new ClsDeserializacaoDelMatchEntrega() { Status = "Não Enviado" };
            }

            return pedido;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        return pedido;
    }


    public static void UpdateDelMatchId(int numConta, string delmatchId)
    {
        try
        {
            using ApplicationDbContext dbPostgres = new ApplicationDbContext();
            ParametrosDoSistema? opcSistema = dbPostgres.parametrosdosistema.ToList().FirstOrDefault();

            string? caminhoBancoAccess = opcSistema.CaminhodoBanco;

            string updateQuery = "UPDATE Sequencia SET DelMatchId = @NovoValor WHERE CONTA = @CONDICAO;";


            string DelMatchId = delmatchId;

            using (OleDbConnection connection = new OleDbConnection(caminhoBancoAccess))
            {
                // Abrindo a conexão
                connection.Open();

                using (OleDbCommand command = new OleDbCommand(updateQuery, connection))
                {
                    // Definindo os parâmetros para a instrução SQL
                    command.Parameters.AddWithValue("@NovoValor1", DelMatchId);
                    command.Parameters.AddWithValue("@CONDICAO", numConta);

                    // Executando o comando UPDATE
                    command.ExecuteNonQuery();
                }
            }


        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "ERRO NO UPDATE DELMATCHID");
        }
    }

    public static Sequencia PesquisaClientesNoCadastro(string? telefone)
    {
        Sequencia sequencia = new Sequencia();
        try
        {
            using ApplicationDbContext dbPostgres = new ApplicationDbContext();
            ParametrosDoSistema? opcSistema = dbPostgres.parametrosdosistema.ToList().FirstOrDefault();

            string? caminhoBancoAccess = opcSistema.CaminhodoBanco.Replace("CONTAS", "CADASTROS");

            string SqlSelectIntoCadastros = $"SELECT * FROM Clientes WHERE TRIM(TELEFONE) = '{telefone}'";

            using (OleDbConnection connection = new OleDbConnection(caminhoBancoAccess))
            {
                connection.Open();


                using (OleDbCommand selectCommand = new OleDbCommand(SqlSelectIntoCadastros, connection))
                {
                    selectCommand.Parameters.AddWithValue("@TELEFONE", telefone);

                    // Executar a consulta SELECT
                    using (OleDbDataReader reader = selectCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            sequencia.Customer.Name = reader["NOME"].ToString();
                            sequencia.Customer.Phone = reader["TELEFONE"].ToString();
                            sequencia.Customer.TaxPayerIdentificationNumber = "CPF"; //falta terminar

                            sequencia.DeliveryAddress.FormattedAddress = reader["ENDERECO"].ToString();
                            sequencia.DeliveryAddress.Country = "BR";
                            sequencia.DeliveryAddress.State = reader["ESTADO"].ToString();
                            sequencia.DeliveryAddress.City = reader["CIDADE"].ToString();
                            sequencia.DeliveryAddress.Neighborhood = reader["BAIRRO"].ToString();
                            sequencia.DeliveryAddress.StreetName = reader["ENDERECO"].ToString();
                            sequencia.DeliveryAddress.StreetNumber = "";                        //falta terminar
                            sequencia.DeliveryAddress.PostalCode = reader["CEP"].ToString() == null ? " " : reader["CEP"].ToString();
                            sequencia.DeliveryAddress.Complement = reader["REFERE"].ToString();

                            sequencia.DeliveryAddress.Coordinates.Latitude = 0;
                            sequencia.DeliveryAddress.Coordinates.Longitude = 0;
                        }
                        return sequencia;
                    }

                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString(), "Erro Ao listar Cliente Cadastrado");
        }
        return sequencia;
    }


    public async static void EnviaPedidosAut()
    {
        try
        {
            using ApplicationDbContext db = new ApplicationDbContext();
            ParametrosDoSistema? Configuracoes = db.parametrosdosistema.ToList().FirstOrDefault();

            if (Configuracoes.EnviaPedidoAut)
            {
                List<Sequencia> pedidosAbertos = DelMatch.ListarPedidosAbertos();
                int contagemdepedidos = pedidosAbertos.Count;
                List<Sequencia> ItensAEnviarDelMach = FormDePedidosAbertos.ItensAEnviarDelMach;

                if (contagemdepedidos > 0)
                {
                    foreach (var item in pedidosAbertos)
                    {
                        ItensAEnviarDelMach.Add(item);
                    }

                }

                if (ItensAEnviarDelMach.Count() > 0)
                {

                    foreach (var item in ItensAEnviarDelMach)
                    {
                        string jsonContent = JsonConvert.SerializeObject(item);
                        await DelMatch.GerarPedido(jsonContent);
                        DelMatch.UpdateDelMatchId(item.numConta, item.ShortReference);
                    }

                    ItensAEnviarDelMach.Clear();
                }
            }

        }
        catch (Exception ex)
        {
            MessageBox.Show("Erro ao enviar pedidos automatico para delmatch", "Ops");
        }
    }


}
