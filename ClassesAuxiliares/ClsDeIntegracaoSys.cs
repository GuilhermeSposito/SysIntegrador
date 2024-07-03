using SysIntegradorApp.data;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoDelmatch;
using SysIntegradorApp.ClassesAuxiliares.logs;
using SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoOnPedido;
using SysIntegradorApp.ClassesDeConexaoComApps;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography.X509Certificates;
using SysIntegradorApp.data.InterfaceDeContexto;

namespace SysIntegradorApp.ClassesAuxiliares;

public class ClsDeIntegracaoSys
{
    public static string? CaminhoBaseSysMenu { get; set; } = ApplicationDbContext.RetornaCaminhoBaseSysMenu();

    public static async Task<int> IntegracaoSequencia(string? mesa, //COMEÇO DOS PARÂMETROS DO MÉTODO
     float cortesia,
     float taxaEntrega,
     float taxaMotoboy,
     string? dtInicio,
     string? hrInicio,
     string? contatoNome,
     string? usuario,
     string? dataSaida,
     string? hrSaida,
     string? obsConta1,
     string? iFoodPedidoID,
     string? obsConta2,
     string? referencia,
     string? endEntrega = "RETIRADA",
     string? bairEntrega = "RETIRADA",
     string? entregador = "RETIRADA",
     bool eIfood = false,
     bool eDelMatch = false,
     bool eOnpedido = false
     ) //método que está sendo usado para integrar a tabela contas do banco de dados com a tabela de pedido do SysIntegrador
    {

        int ultimoNumeroConta = 0;
        try
        {
            string banco = CaminhoBaseSysMenu; //@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\gui-c\OneDrive\Área de Trabalho\SysIntegrador\CONTAS.mdb";
            List<int> numerosSequencia = new List<int>();
            numerosSequencia.Clear();

            using (OleDbConnection connection = new OleDbConnection(banco))
            {
                connection.Open();

                string sqlInsert = $"INSERT INTO Sequencia (MESA, STATUS,CORTESIA ,TAXAENTREGA,TAXAMOTOBOY, DTINICIO, HRINICIO, ENDENTREGA, BAIENTREGA, REFENTREGA ,CONTATO, ENTREGADOR, USUARIO, DTSAIDA, HRSAIDA, OBSCONTA1, OBSCONTA2 ,iFoodPedidoID) VALUES ( ?, ?,?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?,?)";

                using (OleDbCommand command = new OleDbCommand(sqlInsert, connection))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.ForegroundColor = ConsoleColor.White;
                    // Parâmetros para a consulta SQL
                    command.Parameters.AddWithValue("@MESA", mesa);
                    command.Parameters.AddWithValue("@STATUS", "P");
                    command.Parameters.AddWithValue("@CORTESIA", cortesia);
                    command.Parameters.AddWithValue("@TAXAENTREGA", taxaEntrega);
                    command.Parameters.AddWithValue("@TAXAMOTOBOY", taxaMotoboy);
                    command.Parameters.AddWithValue("@DTINICIO", dtInicio.Replace("-", "/"));
                    command.Parameters.AddWithValue("@HRINICIO", hrInicio);
                    command.Parameters.AddWithValue("@ENDENTREGA", endEntrega); //se vier WEBB aqui vai ser null
                    command.Parameters.AddWithValue("@BAIENTREGA", bairEntrega);//se vier WEBB aqui vai ser null
                    command.Parameters.AddWithValue("@REFENTREGA", referencia);//se vier WEBB aqui vai ser null
                    command.Parameters.AddWithValue("@CONTATO", contatoNome);
                    command.Parameters.AddWithValue("@ENTREGADOR", entregador); //se vier WEBB aqui vai ser null
                    command.Parameters.AddWithValue("@USUARIO", usuario);
                    command.Parameters.AddWithValue("@DTSAIDA", dataSaida.Replace("-", "/"));
                    command.Parameters.AddWithValue("@HRSAIDA", hrSaida);
                    command.Parameters.AddWithValue("@OBSCONTA1", obsConta1);
                    command.Parameters.AddWithValue("@OBSCONTA2", obsConta2);
                    command.Parameters.AddWithValue("@iFoodPedidoID", iFoodPedidoID);


                    // Executa o comando SQL
                    int rowsAffected = command.ExecuteNonQuery();


                    if (rowsAffected > 0)
                    {
                        //Se a inserção foi feita com sucesso, nós vamos pegar o número da sequencia
                        string sqlQuery = "SELECT * FROM Sequencia WHERE iFoodPedidoID = @IFOODPEDIDOID";

                        using (OleDbCommand comando = new OleDbCommand(sqlQuery, connection))
                        {
                            comando.Parameters.AddWithValue("@IFOODPEDIDOID", iFoodPedidoID);

                            using (OleDbDataReader reader = comando.ExecuteReader())
                            {
                                if (reader.HasRows)
                                {
                                    while (reader.Read())
                                    {
                                        ultimoNumeroConta = Convert.ToInt32(reader["CONTA"].ToString());
                                    }

                                    if (eIfood)
                                    {
                                        string query = "UPDATE Sequencia SET IsIFood = ?, PEDWEB = ? WHERE CONTA = ?";
                                        using (OleDbCommand ComandoMudaIsIfood = new OleDbCommand(query, connection))
                                        {
                                            OleDbParameter paramIsIFood = new OleDbParameter("@IsIFood", OleDbType.Boolean);
                                            paramIsIFood.Value = true;

                                            OleDbParameter paramConta = new OleDbParameter("@Conta", OleDbType.VarChar);
                                            paramConta.Value = ultimoNumeroConta.ToString(); // Assuming ultimoNumeroConta is a string

                                            OleDbParameter paramPedWeb = new OleDbParameter("@PEDWEB", "IFOOD");


                                            ComandoMudaIsIfood.Parameters.Add(paramIsIFood);
                                            ComandoMudaIsIfood.Parameters.Add(paramPedWeb);
                                            ComandoMudaIsIfood.Parameters.Add(paramConta);

                                            ComandoMudaIsIfood.ExecuteNonQuery();
                                        }
                                    }

                                    if (eDelMatch)
                                    {
                                        string query = "UPDATE Sequencia SET PEDWEB = ? WHERE CONTA = ?";
                                        using (OleDbCommand ComandoMudaIsIfood = new OleDbCommand(query, connection))
                                        {

                                            OleDbParameter paramConta = new OleDbParameter("@CONTA", OleDbType.VarChar);
                                            paramConta.Value = ultimoNumeroConta.ToString(); // Assuming ultimoNumeroConta is a string

                                            OleDbParameter paramPedWeb = new OleDbParameter("@PEDWEB", OleDbType.VarChar);
                                            paramPedWeb.Value = "DELMATCH";

                                            ComandoMudaIsIfood.Parameters.Add(paramPedWeb);
                                            ComandoMudaIsIfood.Parameters.Add(paramConta);

                                            ComandoMudaIsIfood.ExecuteNonQuery();
                                        }

                                    }

                                    if (eOnpedido)
                                    {
                                        string query = "UPDATE Sequencia SET PEDWEB = ? WHERE CONTA = ?";
                                        using (OleDbCommand ComandoMudaIsIfood = new OleDbCommand(query, connection))
                                        {

                                            OleDbParameter paramConta = new OleDbParameter("@CONTA", OleDbType.VarChar);
                                            paramConta.Value = ultimoNumeroConta.ToString();

                                            OleDbParameter paramPedWeb = new OleDbParameter("@PEDWEB", "ONPEDIDO");

                                            ComandoMudaIsIfood.Parameters.Add(paramPedWeb);
                                            ComandoMudaIsIfood.Parameters.Add(paramConta);

                                            ComandoMudaIsIfood.ExecuteNonQuery();
                                        }
                                    }

                                    return ultimoNumeroConta;
                                } // fechamento if hasRows
                            }   //fechamento terceiro using
                        }
                    } //fechamento de chave do if RowsAfected
                }//fechamento chave segundo using 
            }//fechamento chave primeiro using
        }// fechamento chave if
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
            MessageBox.Show("Erro de inserção", "ERRO AO INSERIR O PEDIDO NO BANCO DE DADOS DO ACCESS");
        }

        return ultimoNumeroConta;
    }

    public static void UpdateMeiosDePagamentosSequencia(Payments pagamento, int numConta)
    {
        try
        {
            using ApplicationDbContext dbPostgres = new ApplicationDbContext();
            ParametrosDoSistema? opcSistema = dbPostgres.parametrosdosistema.ToList().FirstOrDefault();

            string? caminhoBancoAccess = opcSistema.CaminhodoBanco;


            foreach (Methods pagamentoAtual in pagamento.methods)
            {
                using (OleDbConnection connection = new OleDbConnection(caminhoBancoAccess))
                {
                    // Abrindo a conexão
                    connection.Open();
                    string? tipoPagamento = "PAGCRT";


                    switch (pagamentoAtual.method)
                    {
                        case "CREDIT":
                            tipoPagamento = "PAGCRT";
                            break;
                        case "MEAL_VOUCHER":
                            tipoPagamento = "VOUCHER";
                            break;
                        case "DEBIT":
                            tipoPagamento = "PAGCRT";
                            break;
                        case "PIX":
                            tipoPagamento = "PAGCRT";
                            break;
                        case "CASH":
                            tipoPagamento = "PAGDNH";
                            break;
                        case "BANK_PAY":
                            tipoPagamento = "PAGCRT";
                            break;
                        case "FOOD_VOUCHER":
                            tipoPagamento = "PAGCRT";
                            break;
                        case "DIN":
                            tipoPagamento = "PAGDNHDELMATCH";
                            break;
                        case "DEB":
                            tipoPagamento = "PAGCRT";
                            break;
                        case "Dinheiro":
                            tipoPagamento = "PAGDNH";
                            break;
                        default:
                            tipoPagamento = "PAGCRT";
                            break;
                    }

                    if (pagamentoAtual.type == "ONLINE")
                    {
                        string updateQuery = $"UPDATE Sequencia SET PAGONLINE = @NovoValor WHERE CONTA = @CONDICAO;";
                        double valor = pagamentoAtual.value;

                        using (OleDbCommand command = new OleDbCommand(updateQuery, connection))
                        {

                            // Definindo os parâmetros para a instrução SQL
                            command.Parameters.AddWithValue($"@NovoValor", valor);
                            command.Parameters.AddWithValue("@CONDICAO", numConta);

                            // Executando o comando UPDATE
                            command.ExecuteNonQuery();
                        }
                        continue;
                    }


                    if (tipoPagamento == "PAGCRT")
                    {
                        string updateQuery = $"UPDATE Sequencia SET PAGCRT = @NovoValor WHERE CONTA = @CONDICAO;";
                        double valor = pagamentoAtual.value;

                        using (OleDbCommand command = new OleDbCommand(updateQuery, connection))
                        {

                            // Definindo os parâmetros para a instrução SQL
                            command.Parameters.AddWithValue($"@NovoValor", valor);
                            command.Parameters.AddWithValue("@CONDICAO", numConta);

                            // Executando o comando UPDATE
                            command.ExecuteNonQuery();
                        }
                        continue;
                    }

                    if (tipoPagamento == "PAGDNH")
                    {
                        string updateQuery = $"UPDATE Sequencia SET PAGDNH = @Valor, TROCO = @ValorTroco, TROCOENTREGA = @ValorTrocoEntrega WHERE CONTA = @CONDICAO;";
                        double valor = pagamentoAtual.value;
                        double troco = 0.0;

                        if (pagamentoAtual.cash.changeFor > 0)
                        {
                            valor = pagamentoAtual.cash.changeFor;
                            troco = pagamentoAtual.cash.changeFor - pagamentoAtual.value;
                        }


                        using (OleDbCommand command = new OleDbCommand(updateQuery, connection))
                        {

                            // Definindo os parâmetros para a instrução SQL
                            command.Parameters.AddWithValue($"@Valor", valor);
                            command.Parameters.AddWithValue("@ValorTroco", troco);
                            command.Parameters.AddWithValue("@ValorTrocoEntrega", troco);
                            command.Parameters.AddWithValue("@CONDICAO", numConta);

                            // Executando o comando UPDATE
                            command.ExecuteNonQuery();
                        }
                        continue;
                    }

                    if (tipoPagamento == "PAGDNHDELMATCH")
                    {
                        tipoPagamento = "PAGDNH";
                        string updateQuery = $"UPDATE Sequencia SET PAGDNH = @Valor, TROCO = @ValorTroco, TROCOENTREGA = @ValorTrocoEntrega WHERE CONTA = @CONDICAO;";
                        double valor = pagamentoAtual.value;
                        double troco = 0.0;

                        if (pagamentoAtual.cash.changeFor > 0)
                        {
                            valor = pagamentoAtual.value;
                            troco = pagamentoAtual.cash.changeFor;
                        }


                        using (OleDbCommand command = new OleDbCommand(updateQuery, connection))
                        {

                            // Definindo os parâmetros para a instrução SQL
                            command.Parameters.AddWithValue($"@Valor", valor);
                            command.Parameters.AddWithValue("@ValorTroco", troco);
                            command.Parameters.AddWithValue("@ValorTrocoEntrega", troco);
                            command.Parameters.AddWithValue("@CONDICAO", numConta);

                            // Executando o comando UPDATE
                            command.ExecuteNonQuery();
                        }
                        continue;
                    }

                    if (tipoPagamento == "VOUCHER")
                    {
                        string updateQuery = $"UPDATE Sequencia SET VOUCHER = @NovoValor WHERE CONTA = @CONDICAO;";
                        double valor = pagamentoAtual.value;

                        using (OleDbCommand command = new OleDbCommand(updateQuery, connection))
                        {

                            // Definindo os parâmetros para a instrução SQL
                            command.Parameters.AddWithValue($"@NovoValor", valor);
                            command.Parameters.AddWithValue("@CONDICAO", numConta);

                            // Executando o comando UPDATE
                            command.ExecuteNonQuery();
                        }
                        continue;
                    }


                }
            }


        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Erro dentro do insere valores");
        }
    }

    public static async void IntegracaoContas(int conta, //numero
      string? mesa, //texto curto 
      float qtdade, //numero
      string? codCarda1, //texto curto 4 letras
      string? codCarda2, // texto curto 4 letras
      string? codCarda3, //texto curto 4 letras
      string? tamanho,  //texto curto 1 letra
      string? descarda, // texto curto 31 letras
      float valorUnit, //moeda
      float valorTotal, // moeda
      string dataInicio, //data/hora
      string horaInicio,  //data/hora
      string? obs1,  //texto curto 80 letras
      string? obs2,  //texto curto 80 letras
      string? obs3,  //texto curto 80 letras
      string? obs4,    //  ||
      string? obs5,    //  ||
      string? obs6,    //  ||
      string? obs7,    //  ||
      string? obs8,    //  ||
      string? obs9,    //  ||
      string? obs10,    //  ||
      string? obs11,    //  ||
      string? obs12,    //  ||
      string? obs13,    //  ||
      string? obs14,    //  ||
      string? obs15,    //  ||
      string? cliente, // texto curto 40 letras
      string? telefone, // texto curto 14 letras
      string? impComanda, // texto curto 3 letras
      string? ImpComanda2, // texto curto 3 letras
      float qtdComanda, //numero duplo 
      string? usuario = "CAIXA",
      string? status = "P",
      bool pedidoOnLineMesa = false,
      string? idPedido = " "
      )
    { //aqui começa o código para inserção na tabela CONTAS
        try
        {
            string banco = CaminhoBaseSysMenu;//@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\gui-c\OneDrive\Área de Trabalho\SysIntegrador\CONTAS.mdb";

            if (telefone.Length > 14)
            {
                telefone = telefone.Replace(" ", "");
            }

            using (OleDbConnection connection = new OleDbConnection(banco))
            {
                connection.Open();

                string sqlInsert = $"INSERT INTO Contas (CONTA,MESA,QTDADE,CODCARDA1,CODCARDA2,CODCARDA3,TAMANHO,DESCARDA,VALORUNIT,VALORTOTAL,DATAINICIO,HORAINICIO,OBS1,OBS2,OBS3,OBS4,OBS5,OBS6,OBS7,OBS8,OBS9,OBS10,OBS11,OBS12,OBS13,OBS14,OBS15,CLIENTE,STATUS,TELEFONE,IMPCOMANDA,IMPCOMANDA2,QTDCOMANDA,USUARIO ) VALUES (?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?)";

                using (OleDbCommand command = new OleDbCommand(sqlInsert, connection))
                {
                    // Parâmetros para a consulta SQL
                    command.Parameters.AddWithValue("@CONTA", conta);
                    command.Parameters.AddWithValue("@MESA", mesa);
                    command.Parameters.AddWithValue("@QTDADE", qtdade);
                    command.Parameters.AddWithValue("@CODCARDA1", codCarda1);
                    command.Parameters.AddWithValue("@CODCARDA2", codCarda2);
                    command.Parameters.AddWithValue("@CODCARDA3", codCarda3);
                    command.Parameters.AddWithValue("@TAMANHO", tamanho);
                    command.Parameters.AddWithValue("@DESCARDA", descarda.Length > 31 ? descarda.Substring(0,31) : descarda);
                    command.Parameters.AddWithValue("@VALORUNIT", valorUnit); //se vier WEBB aqui vai ser null
                    command.Parameters.AddWithValue("@VALORTOTAL", valorTotal);//se vier WEBB aqui vai ser null
                    command.Parameters.AddWithValue("@DATAINICIO", dataInicio);
                    command.Parameters.AddWithValue("@HORAINICIO", horaInicio); //se vier WEBB aqui vai ser null
                    command.Parameters.AddWithValue("@OBS1", obs1);
                    command.Parameters.AddWithValue("@OBS2", obs2);
                    command.Parameters.AddWithValue("@OBS3", obs3);
                    command.Parameters.AddWithValue("@OBS4", obs4);
                    command.Parameters.AddWithValue("@OBS5", obs5);
                    command.Parameters.AddWithValue("@OBS6", obs6);
                    command.Parameters.AddWithValue("@OBS7", obs7);
                    command.Parameters.AddWithValue("@OBS8", obs8);
                    command.Parameters.AddWithValue("@OBS9", obs9);
                    command.Parameters.AddWithValue("@OBS10", obs10);
                    command.Parameters.AddWithValue("@OBS11", obs11);
                    command.Parameters.AddWithValue("@OBS12", obs12);
                    command.Parameters.AddWithValue("@OBS13", obs13);
                    command.Parameters.AddWithValue("@OBS14", obs14);
                    command.Parameters.AddWithValue("@OBS15", obs15);
                    command.Parameters.AddWithValue("@CLIENTE", cliente.Length > 40 ? cliente.Substring(0,40) : cliente);
                    command.Parameters.AddWithValue("@STATUS", status);
                    command.Parameters.AddWithValue("@TELEFONE", telefone.Length > 14 ? telefone.Substring(0,14) : telefone);
                    command.Parameters.AddWithValue("@IMPCOMANDA", impComanda);
                    command.Parameters.AddWithValue("@IMPCOMANDA2", ImpComanda2);
                    command.Parameters.AddWithValue("@QTDCOMANDA", qtdComanda);
                    command.Parameters.AddWithValue("@USUARIO", usuario);

                    // Executa o comando SQL
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        if (pedidoOnLineMesa)
                        {
                            string? sqlUpdate = "UPDATE Contas SET PEDIDOONLINEID = @PEDIDOONLINEID WHERE MESA = @MESA AND STATUS = @STATUS";

                            using (OleDbCommand commando = new OleDbCommand(sqlUpdate, connection))
                            {
                                commando.Parameters.AddWithValue("@PEDIDOONLINEID", idPedido); //PEDIDOONLINEID
                                commando.Parameters.AddWithValue("@MESA", mesa);
                                commando.Parameters.AddWithValue("@STATUS", "A");

                                commando.ExecuteNonQuery();
                            }
                        }

                    }//fechamento chave segundo using 
                }//fechamento chave primeiro using
            }
        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
            MessageBox.Show("Erro de integração com o contas", "Ops");
        }
    }


    public static async void IntegracaoPagCartao(string? metodo, int NumContas, float valor, string type, string app)
    {
        string? cartao = "";
        string? tipo = "";
        try
        {
            string banco = CaminhoBaseSysMenu;//@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\gui-c\OneDrive\Área de Trabalho\SysIntegrador\CONTAS.mdb";
            using ApplicationDbContext dbPostgres = new ApplicationDbContext();
            ParametrosDoSistema? opcSistema = dbPostgres.parametrosdosistema.ToList().FirstOrDefault();

            string? caminhoBancoAccess = opcSistema.CaminhodoBanco.Replace("CONTAS", "CADASTROS");

            using (OleDbConnection connection = new OleDbConnection(caminhoBancoAccess))
            {
                connection.Open();

                string? tipoPagamento = DefinePagamento(metodo, app);//pedidoCompleto.payments.methods[0].method);


                string SqlSelectIntoCadastros = $"SELECT * FROM CARTAO WHERE NOME = {tipoPagamento}";


                using (OleDbCommand selectCommand = new OleDbCommand(SqlSelectIntoCadastros, connection))
                {
                    selectCommand.Parameters.AddWithValue("@NOME", tipoPagamento);

                    // Executar a consulta SELECT
                    using (OleDbDataReader reader = selectCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cartao = reader["NOME"].ToString();
                            tipo = reader["TIPO"].ToString();
                        }
                    }
                }
            }

            if (type != "ONLINE" || type != "online")
            {
                using (OleDbConnection connection = new OleDbConnection(banco))
                {
                    connection.Open();

                    float valorPagamento = valor;//pedidoCompleto.payments.methods[0].value;

                    string sqlInsert = $"INSERT INTO PagCartao (CONTA, CARTAO, TIPO, VALOR) VALUES (?,?,?,?)";

                    var CartaoFinal = cartao == null || cartao == "" ? " " : cartao;
                    var TipoFinal = tipo == null || tipo == "" ? " " : tipo;

                    using (OleDbCommand command = new OleDbCommand(sqlInsert, connection))
                    {
                        // Parâmetros para a consulta SQL
                        command.Parameters.AddWithValue("@CONTA", NumContas);
                        command.Parameters.AddWithValue("@CARTAO", CartaoFinal);
                        command.Parameters.AddWithValue("@TIPO", TipoFinal);
                        command.Parameters.AddWithValue("@VALOR", valorPagamento);


                        // Executa o comando SQL
                        int rowsAffected = command.ExecuteNonQuery();

                    }//fechamento chave segundo using 
                }//fechamento chave primeiro using
            }
        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
            MessageBox.Show(ex.Message, "Ops");
        }
    }

    public static bool ProcuraCliente(string? telefone)
    {
        bool existeCliente = false;
        try
        {
            string banco = CaminhoBaseSysMenu;//@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\gui-c\OneDrive\Área de Trabalho\SysIntegrador\CONTAS.mdb";
            using ApplicationDbContext dbPostgres = new ApplicationDbContext();
            ParametrosDoSistema? opcSistema = dbPostgres.parametrosdosistema.ToList().FirstOrDefault();

            string? caminhoBancoAccess = opcSistema.CaminhodoBanco.Replace("CONTAS", "CADASTROS");

            using (OleDbConnection connection = new OleDbConnection(caminhoBancoAccess))
            {
                connection.Open();

                string sqlSelect = "SELECT COUNT(*) FROM Clientes WHERE TELEFONE = @TELEFONE";


                using (OleDbCommand selectCommand = new OleDbCommand(sqlSelect, connection))
                {
                    selectCommand.Parameters.AddWithValue("@TELEFONE", telefone);

                    // Executar a consulta SELECT
                    int count = (int)selectCommand.ExecuteScalar();
                    existeCliente = count > 0;
                }
                return existeCliente;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString(), "Problema em procurar cliente");
        }
        return existeCliente;
    }

    public static bool ProcuraMesaFechada()
    {
        bool ExisteMesa = false;
        try
        {
            string banco = CaminhoBaseSysMenu;
            using ApplicationDbContext dbPostgres = new ApplicationDbContext();
            ParametrosDoSistema? opcSistema = dbPostgres.parametrosdosistema.ToList().FirstOrDefault();

            string? caminhoBancoAccess = opcSistema.CaminhodoBanco;

            using (OleDbConnection connection = new OleDbConnection(caminhoBancoAccess))
            {
                connection.Open();

                string sqlSelect = "SELECT COUNT(*) FROM ApoMesa";


                using (OleDbCommand selectCommand = new OleDbCommand(sqlSelect, connection))
                {
                    // Executar a consulta SELECT
                    int count = (int)selectCommand.ExecuteScalar();
                    ExisteMesa = count > 0;
                }
                return ExisteMesa;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString(), "Problema em procurar mesa para fechar");
        }
        return ExisteMesa;
    }


    public static async void CadastraCliente(Customer cliente, Delivery entrega)
    {
        try
        {
            string banco = CaminhoBaseSysMenu;//@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\gui-c\OneDrive\Área de Trabalho\SysIntegrador\CONTAS.mdb";
            using ApplicationDbContext dbPostgres = new ApplicationDbContext();
            ParametrosDoSistema? opcSistema = dbPostgres.parametrosdosistema.ToList().FirstOrDefault();

            string? caminhoBancoAccess = opcSistema.CaminhodoBanco.Replace("CONTAS", "CADASTROS");

            using (OleDbConnection connection = new OleDbConnection(caminhoBancoAccess))
            {
                connection.Open();

                string SqlSelectIntoCadastros = "INSERT INTO Clientes (TELEFONE, NOME, ENDERECO, BAIRRO, CIDADE, ESTADO, CEP, REFERE) VALUES (?,?,?,?,?,?,?,?) ";

                string referenciaDoEndereco = entrega.deliveryAddress.reference == null || entrega.deliveryAddress.reference == "" ? " " : entrega.deliveryAddress.reference;


                using (OleDbCommand command = new OleDbCommand(SqlSelectIntoCadastros, connection))
                {
                    command.Parameters.AddWithValue("@TELEFONE", cliente.phone.localizer);
                    command.Parameters.AddWithValue("@NOME", cliente.name);
                    command.Parameters.AddWithValue("@ENDERECO", entrega.deliveryAddress.formattedAddress);
                    command.Parameters.AddWithValue("@BAIRRO", entrega.deliveryAddress.neighborhood);
                    command.Parameters.AddWithValue("@CIDADE", entrega.deliveryAddress.city);
                    command.Parameters.AddWithValue("@ESTADO", "SP");
                    command.Parameters.AddWithValue("@CEP", entrega.deliveryAddress.postalCode != null || entrega.deliveryAddress.postalCode != "" ? entrega.deliveryAddress.postalCode : " ");
                    command.Parameters.AddWithValue("@REFERE", referenciaDoEndereco);


                    // Executa o comando SQL
                    int rowsAffected = command.ExecuteNonQuery();
                }

            }
        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
            MessageBox.Show(ex.ToString(), "Problema em procurar cliente");
        }
    }

    public static async void CadastraClienteOnPedido(SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoOnPedido.Customer cliente, SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoOnPedido.DeliveryOn entrega)
    {
        try
        {
            string banco = CaminhoBaseSysMenu;//@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\gui-c\OneDrive\Área de Trabalho\SysIntegrador\CONTAS.mdb";
            using ApplicationDbContext dbPostgres = new ApplicationDbContext();
            ParametrosDoSistema? opcSistema = dbPostgres.parametrosdosistema.ToList().FirstOrDefault();

            string? caminhoBancoAccess = opcSistema.CaminhodoBanco.Replace("CONTAS", "CADASTROS");

            using (OleDbConnection connection = new OleDbConnection(caminhoBancoAccess))
            {
                connection.Open();

                string SqlSelectIntoCadastros = "INSERT INTO Clientes (TELEFONE, NOME, ENDERECO, BAIRRO, CIDADE, ESTADO, CEP, REFERE) VALUES (?,?,?,?,?,?,?,?) ";

                string referenciaDoEndereco = entrega.DeliveryAddressON.Complement == null || entrega.DeliveryAddressON.Complement == "" ? " " : entrega.DeliveryAddressON.Complement;

                string TelefoneCliente = $"({cliente.PhoneOn.Extension}){cliente.PhoneOn.Number}";

                using (OleDbCommand command = new OleDbCommand(SqlSelectIntoCadastros, connection))
                {
                    command.Parameters.AddWithValue("@TELEFONE", TelefoneCliente);
                    command.Parameters.AddWithValue("@NOME", cliente.Name);
                    command.Parameters.AddWithValue("@ENDERECO", entrega.DeliveryAddressON.FormattedAddress);
                    command.Parameters.AddWithValue("@BAIRRO", entrega.DeliveryAddressON.District);
                    command.Parameters.AddWithValue("@CIDADE", entrega.DeliveryAddressON.City);
                    command.Parameters.AddWithValue("@ESTADO", "SP");
                    command.Parameters.AddWithValue("@CEP", entrega.DeliveryAddressON.PostalCode != null && entrega.DeliveryAddressON.PostalCode != "" ? entrega.DeliveryAddressON.PostalCode : " ");
                    command.Parameters.AddWithValue("@REFERE", referenciaDoEndereco);


                    // Executa o comando SQL
                    int rowsAffected = command.ExecuteNonQuery();
                }

            }
        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
            MessageBox.Show(ex.ToString(), "Problema em procurar cliente");
        }
    }


    public static string DefinePagamento(string? tipoPagamento, string app) //define o nome do pagamento para inserir no pagCartão
    {
        string pagamento = "";

        if (app == "ONPEDIDO")
        {
            bool eDebito = tipoPagamento.Contains("Débito");
            if (eDebito)
            {
                tipoPagamento = "DEBIT";
            }
            bool eCredito = tipoPagamento.Contains("Crédito");
            if (eCredito)
            {
                tipoPagamento = "CREDIT";
            }
            bool eRefeicao = tipoPagamento.Contains("Refeição") || tipoPagamento.Contains("Refeicao");
            if (eRefeicao)
            {
                tipoPagamento = "MEAL_VOUCHER";
            }
        }

        switch (tipoPagamento)
        {
            case "CREDIT":
                pagamento = "Crédito";
                break;
            case "MEAL_VOUCHER":
                pagamento = "Débito";
                break;
            case "DEBIT":
                pagamento = "Débito";
                break;
            case "PIX":
                pagamento = "PIX";
                break;
            case "BANK_PAY":
                pagamento = "Débito";
                break;
            case "FOOD_VOUCHER":
                pagamento = "Débito";
                break;
            case "OTHER":
                pagamento = "Crédito";
                break;
            case "VOUCHER":
                pagamento = "Crédito";
                break;
            default:
                pagamento = "Débito";
                break;
        }

        return pagamento;
    }

    public static bool PesquisaCodCardapio(string? codPdv)
    {
        bool existeProduto = false;
        try
        {
            if (codPdv != null)
            {
                string banco = CaminhoBaseSysMenu;//@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\gui-c\OneDrive\Área de Trabalho\SysIntegrador\CONTAS.mdb";
                using ApplicationDbContext dbPostgres = new ApplicationDbContext();
                ParametrosDoSistema? opcSistema = dbPostgres.parametrosdosistema.ToList().FirstOrDefault();

                string? caminhoBancoAccess = opcSistema.CaminhodoBanco.Replace("CONTAS", "CADASTROS");

                using (OleDbConnection connection = new OleDbConnection(caminhoBancoAccess))
                {
                    connection.Open();

                    string sqlSelect = "SELECT COUNT(*) FROM Cardapio WHERE CODIGO = @CODIGO";

                    using (OleDbCommand selectCommand = new OleDbCommand(sqlSelect, connection))
                    {
                        selectCommand.Parameters.AddWithValue("@CODIGO", codPdv);

                        // Executar a consulta SELECT
                        int count = (int)selectCommand.ExecuteScalar();
                        existeProduto = count > 0;
                    }
                    return existeProduto;
                }
            }
            else
            {
                return existeProduto = false;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString(), "Erro ao pesquisar pelo codigo do cardapio");
        }
        return existeProduto;
    }


    public static string NomeProdutoCardapio(string codCardapio)
    {
        string? NomeProduto = null;
        try
        {
            string banco = CaminhoBaseSysMenu;//@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\gui-c\OneDrive\Área de Trabalho\SysIntegrador\CONTAS.mdb";
            using ApplicationDbContext dbPostgres = new ApplicationDbContext();
            ParametrosDoSistema? opcSistema = dbPostgres.parametrosdosistema.ToList().FirstOrDefault();

            string? caminhoBancoAccess = opcSistema.CaminhodoBanco.Replace("CONTAS", "CADASTROS");

            using (OleDbConnection connection = new OleDbConnection(caminhoBancoAccess))
            {
                connection.Open();

                string SqlSelectIntoCadastros = "SELECT * FROM Cardapio WHERE CODIGO = @CODIGO";


                using (OleDbCommand selectCommand = new OleDbCommand(SqlSelectIntoCadastros, connection))
                {
                    selectCommand.Parameters.AddWithValue("@CODIGO", codCardapio);

                    // Executar a consulta SELECT
                    using (OleDbDataReader reader = selectCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            NomeProduto = reader["DESCRICAO"].ToString();
                        }
                    }
                    return NomeProduto;
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString(), "Erro ao definir nome do produto");
        }
        return NomeProduto;
    }


    public static ClsApoioFechamanetoDeMesa MesasFechadas()
    {
        ClsApoioFechamanetoDeMesa ClsApoioFechamanetoDeMesa = new ClsApoioFechamanetoDeMesa();
        try
        {
            string banco = CaminhoBaseSysMenu;//@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\gui-c\OneDrive\Área de Trabalho\SysIntegrador\CONTAS.mdb";
            using ApplicationDbContext dbPostgres = new ApplicationDbContext();
            ParametrosDoSistema? opcSistema = dbPostgres.parametrosdosistema.ToList().FirstOrDefault();

            string? caminhoBancoAccess = opcSistema.CaminhodoBanco;

            using (OleDbConnection connection = new OleDbConnection(caminhoBancoAccess))
            {
                connection.Open();

                string SqlSelectIntoCadastros = "SELECT * FROM ApoMesa";


                using (OleDbCommand selectCommand = new OleDbCommand(SqlSelectIntoCadastros, connection))
                {
                    // Executar a consulta SELECT
                    using (OleDbDataReader reader = selectCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Mesa mesa = new Mesa();

                            mesa.MESA = reader["MESA"].ToString();
                            mesa.PedidoID = reader["PEDIDOONLINEID"].ToString();

                            ClsApoioFechamanetoDeMesa.Mesas.Add(mesa);
                        }
                    }
                }

                string? DeleteString = "DELETE FROM ApoMesa";

                using (OleDbCommand DeleteCommand = new OleDbCommand(DeleteString, connection))
                {
                    int rowsAffected = DeleteCommand.ExecuteNonQuery();
                }


                return ClsApoioFechamanetoDeMesa;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString(), "Erro ao definir nome do produto");
        }
        return ClsApoioFechamanetoDeMesa;
    }


    public static ClsDeSuporteParaImpressaoDosItens DefineCaracteristicasDoItemOnPedido(itemsOn item, bool comanda = false)
    {
        string? NomeProduto = "";
        ClsDeSuporteParaImpressaoDosItens ClasseDeSuporte = new ClsDeSuporteParaImpressaoDosItens();

        //Função que entra caso sejá pizza ou lanche ou porção, não mudei o nome do booleano pq já estava estruturado o cod 

        bool ePizza = item.externalCode == "G" || item.externalCode == "M" || item.externalCode == "P" || item.externalCode == "B" || item.externalCode == "LAN" || item.externalCode == "PRC"? true : false;

        string? ObsDoItem = " ";

        if (item.observations != null && item.observations != "")
        {
            if (item.observations.Length > 80)
            {
                ObsDoItem = item.observations.Substring(0, 80);
            }
            else
            {
                ObsDoItem = item.observations;
            }
        }

        ClasseDeSuporte.ObsDoItem = ObsDoItem;

        if (ePizza)
        {
            string obs = item.observations == null || item.observations == "" ? " " : item.observations.ToString();
            string externalCode1 = " ";
            string externalCode2 = " ";
            string externalCode3 = " ";

            string? ePizza1 = null;
            string? ePizza2 = null;
            string? ePizza3 = null;

            string? obs1 = " ";
            string? obs2 = " ";
            string? obs3 = " ";
            string? obs4 = " ";
            string? obs5 = " ";
            string? obs6 = " ";
            string? obs7 = " ";
            string? obs8 = " ";
            string? obs9 = " ";
            string? obs10 = " ";
            string? obs11 = " ";
            string? obs12 = " ";
            string? obs13 = " ";
            string? obs14 = " ";

            foreach (var option in item.Options)
            {
                bool eIncremento = option.externalCode.Length == 3 ? true : false;

                if (!option.externalCode.Contains("m") && !eIncremento && ePizza1 == null)
                {
                    ePizza1 = option.externalCode == "" ? " " : option.externalCode;
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(option.externalCode);

                    if (pesquisaProduto)
                    {
                        NomeProduto = $"{item.quantity}X " + ClsDeIntegracaoSys.NomeProdutoCardapio(option.externalCode);
                        externalCode1 = option.externalCode;
                    }
                    else
                    {
                        NomeProduto += option.name;
                    }
                    continue;
                }

                if (!option.externalCode.Contains("m") && !eIncremento && ePizza2 == null)
                {
                    ePizza2 = option.externalCode == "" ? " " : option.externalCode;
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(option.externalCode);

                    if (pesquisaProduto)
                    {
                        NomeProduto += " / " + ClsDeIntegracaoSys.NomeProdutoCardapio(option.externalCode);
                        externalCode2 = option.externalCode;
                    }
                    else
                    {
                        NomeProduto += " / " + option.name;
                    }
                    continue;
                }

                if (!option.externalCode.Contains("m") && !eIncremento && ePizza3 == null)
                {
                    ePizza3 = option.externalCode == "" ? " " : option.externalCode;
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(option.externalCode);

                    if (pesquisaProduto)
                    {
                        NomeProduto += " / " + ClsDeIntegracaoSys.NomeProdutoCardapio(option.externalCode);
                        externalCode3 = option.externalCode;
                    }
                    else
                    {
                        NomeProduto += " / " + option.name;
                    }
                    continue;
                }

            }

            //aqui só define o tamanho se for pizza, se não for ele fica U de uitario

            ClasseDeSuporte.Tamanho = item.externalCode == "G" || item.externalCode == "M" || item.externalCode == "P" || item.externalCode == "B" ? item.externalCode : "U";
            ClasseDeSuporte.ExternalCode1 = externalCode1;
            ClasseDeSuporte.ExternalCode2 = externalCode2;
            ClasseDeSuporte.ExternalCode3 = externalCode3;

            foreach (var opcao in item.Options)
            {
                bool eIncremento = opcao.externalCode.Length == 3 ? true : false;

                if (obs1 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                    if (pesquisaProduto && (opcao.externalCode.Contains("m") || eIncremento))
                    {
                        obs1 = $"{opcao.quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode)}";
                        ClasseDeSuporte.Observações.Add(obs1);
                    }
                    else if (opcao.externalCode.Contains("m") || eIncremento)
                    {
                        string precoProduto = $"- {opcao.TotalPrice.Value.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs1 = $"{opcao.name} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs1);
                    }

                    continue;
                }

                if (obs2 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                    if (pesquisaProduto && opcao.externalCode.Contains("m") && eIncremento)
                    {
                        obs2 = $"{opcao.quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode)}";
                        ClasseDeSuporte.Observações.Add(obs2);
                    }
                    else if (opcao.externalCode.Contains("m") || eIncremento)
                    {
                        string precoProduto = $"- {opcao.TotalPrice.Value.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs2 = $"{opcao.name} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs2);

                    }

                    continue;
                }

                if (obs3 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                    if (pesquisaProduto && opcao.externalCode.Contains("m") && eIncremento)
                    {
                        obs3 = $"{opcao.quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode)}";
                        ClasseDeSuporte.Observações.Add(obs3);

                    }
                    else if (opcao.externalCode.Contains("m") || eIncremento)
                    {
                        string precoProduto = $"- {opcao.TotalPrice.Value.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs3 = $"{opcao.name} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs3);

                    }

                    continue;
                }

                if (obs4 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                    if (pesquisaProduto && opcao.externalCode.Contains("m") && eIncremento)
                    {
                        obs4 = $"{opcao.quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode)}";
                        ClasseDeSuporte.Observações.Add(obs4);

                    }
                    else if (opcao.externalCode.Contains("m") || eIncremento)
                    {
                        string precoProduto = $"-  {opcao.TotalPrice.Value.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs4 = $"{opcao.name} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs4);

                    }

                    continue;
                }

                if (obs5 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                    if (pesquisaProduto && opcao.externalCode.Contains("m") && eIncremento)
                    {
                        obs5 = $"{opcao.quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode)}";
                        ClasseDeSuporte.Observações.Add(obs5);

                    }
                    else if (opcao.externalCode.Contains("m") || eIncremento)
                    {
                        string precoProduto = $"- {opcao.TotalPrice.Value.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs5 = $"{opcao.name} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs5);

                    }

                    continue;
                }

                if (obs6 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                    if (pesquisaProduto && opcao.externalCode.Contains("m") && eIncremento)
                    {
                        obs6 = $"{opcao.quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode)}";
                        ClasseDeSuporte.Observações.Add(obs6);

                    }
                    else if (opcao.externalCode.Contains("m") || eIncremento)
                    {
                        string precoProduto = $"- {opcao.TotalPrice.Value.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs6 = $"{opcao.name} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs6);

                    }

                    continue;
                }

                if (obs7 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                    if (pesquisaProduto && opcao.externalCode.Contains("m") && eIncremento)
                    {
                        obs7 = $"{opcao.quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode)}";
                        ClasseDeSuporte.Observações.Add(obs7);

                    }
                    else if (opcao.externalCode.Contains("m") || eIncremento)
                    {
                        string precoProduto = $"- {opcao.TotalPrice.Value.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs7 = $"{opcao.name} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs7);

                    }

                    continue;
                }

                if (obs8 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                    if (pesquisaProduto && opcao.externalCode.Contains("m") && eIncremento)
                    {
                        obs8 = $"{opcao.quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode)}";
                        ClasseDeSuporte.Observações.Add(obs8);

                    }
                    else if (opcao.externalCode.Contains("m") || eIncremento)
                    {
                        string precoProduto = $"- {opcao.TotalPrice.Value.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs8 = $"{opcao.name} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs8);

                    }

                    continue;
                }

                if (obs9 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                    if (pesquisaProduto && opcao.externalCode.Contains("m") && eIncremento)
                    {
                        obs9 = $"{opcao.quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode)}";
                        ClasseDeSuporte.Observações.Add(obs9);

                    }
                    else if (opcao.externalCode.Contains("m") || eIncremento)
                    {
                        string precoProduto = $"- {opcao.TotalPrice.Value.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs9 = $"{opcao.name} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs9);

                    }

                    continue;
                }

                if (obs10 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                    if (pesquisaProduto && opcao.externalCode.Contains("m") && eIncremento)
                    {
                        obs10 = $"{opcao.quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode)}";
                        ClasseDeSuporte.Observações.Add(obs10);

                    }
                    else if (opcao.externalCode.Contains("m") || eIncremento)
                    {
                        string precoProduto = $"- {opcao.TotalPrice.Value.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs10 = $"{opcao.name} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs10);

                    }

                    continue;
                }

                if (obs11 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                    if (pesquisaProduto && opcao.externalCode.Contains("m") && eIncremento)
                    {
                        obs11 = $"{opcao.quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode)}";
                        ClasseDeSuporte.Observações.Add(obs11);

                    }
                    else if (opcao.externalCode.Contains("m") || eIncremento)
                    {
                        string precoProduto = $"- {opcao.TotalPrice.Value.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs11 = $"{opcao.name} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs11);
                    }

                    continue;
                }

                if (obs12 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                    if (pesquisaProduto && opcao.externalCode.Contains("m") && eIncremento)
                    {
                        obs12 = $"{opcao.quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode)}";
                        ClasseDeSuporte.Observações.Add(obs12);

                    }
                    else if (opcao.externalCode.Contains("m") || eIncremento)
                    {
                        string precoProduto = $"- {opcao.TotalPrice.Value.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs12 = $"{opcao.name} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs12);

                    }

                    continue;
                }

                if (obs13 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                    if (pesquisaProduto && opcao.externalCode.Contains("m") && eIncremento)
                    {
                        obs13 = $"{opcao.quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode)}";
                        ClasseDeSuporte.Observações.Add(obs13);

                    }
                    else if (opcao.externalCode.Contains("m") || eIncremento)
                    {
                        string precoProduto = $"- {opcao.TotalPrice.Value.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs13 = $"{opcao.name} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs13);

                    }

                    continue;
                }

                if (obs14 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                    if (pesquisaProduto && opcao.externalCode.Contains("m") && eIncremento)
                    {
                        obs14 = $"{opcao.quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode)}";
                        ClasseDeSuporte.Observações.Add(obs14);

                    }
                    else if (opcao.externalCode.Contains("m") || eIncremento)
                    {
                        string precoProduto = $"- {opcao.TotalPrice.Value.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs14 = $"{opcao.name} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs14);
                    }

                    continue;
                }

            }

            ClasseDeSuporte.NomeProduto = NomeProduto;
            ClasseDeSuporte.Obs1 = obs1;
            ClasseDeSuporte.Obs2 = obs2;
            ClasseDeSuporte.Obs3 = obs3;
            ClasseDeSuporte.Obs4 = obs4;
            ClasseDeSuporte.Obs5 = obs5;
            ClasseDeSuporte.Obs6 = obs6;
            ClasseDeSuporte.Obs7 = obs7;
            ClasseDeSuporte.Obs8 = obs8;
            ClasseDeSuporte.Obs9 = obs9;
            ClasseDeSuporte.Obs10 = obs10;
            ClasseDeSuporte.Obs11 = obs11;
            ClasseDeSuporte.Obs12 = obs12;
            ClasseDeSuporte.Obs13 = obs13;
            ClasseDeSuporte.Obs14 = obs14;

            return ClasseDeSuporte;

        }
        else
        {
            string? externalCode = item.externalCode == null || item.externalCode == "" ? " " : item.externalCode;
            string? obs = item.observations == null || item.observations == "" ? " " : item.observations.ToString();

            string? externalCode1 = " ";
            string? externalCode2 = " ";
            string? externalCode3 = " ";

            bool existeProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(externalCode);

            if (existeProduto)
            {
                NomeProduto = ClsDeIntegracaoSys.NomeProdutoCardapio(externalCode);
                externalCode1 = externalCode;
            }
            else
            {
                NomeProduto = item.Name;
            }

            if (item.externalCode == "BB")
            {
                int quantidadeItems = 0;

                foreach (var opcao in item.Options)
                {
                    quantidadeItems += opcao.quantity;
                }

                NomeProduto = $"{item.Name}";//{quantidadeItems}X 
            }


            ClasseDeSuporte.NomeProduto = NomeProduto;

            string? obs1 = " ";
            string? obs2 = " ";
            string? obs3 = " ";
            string? obs4 = " ";
            string? obs5 = " ";
            string? obs6 = " ";
            string? obs7 = " ";
            string? obs8 = " ";
            string? obs9 = " ";
            string? obs10 = " ";
            string? obs11 = " ";
            string? obs12 = " ";
            string? obs13 = " ";
            string? obs14 = " ";


            foreach (var opcao in item.Options)
            {
                if (obs1 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                    if (pesquisaProduto)
                    {
                        obs1 = $"{opcao.quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode)}";
                        ClasseDeSuporte.Observações.Add(obs1);
                    }
                    else
                    {
                        string precoProduto = $"- {opcao.TotalPrice.Value.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs1 = $"{opcao.name} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs1);
                    }

                    continue;
                }

                if (obs2 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                    if (pesquisaProduto)
                    {
                        obs2 = $"{opcao.quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode)}";
                        ClasseDeSuporte.Observações.Add(obs2);
                    }
                    else
                    {
                        string precoProduto = $"- {opcao.TotalPrice.Value.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs2 = $"{opcao.name} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs2);
                    }

                    continue;
                }

                if (obs3 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                    if (pesquisaProduto)
                    {
                        obs3 = $"{opcao.quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode)}";
                        ClasseDeSuporte.Observações.Add(obs3);
                    }
                    else
                    {
                        string precoProduto = $"- {opcao.TotalPrice.Value.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs3 = $"{opcao.name} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs3);
                    }

                    continue;
                }

                if (obs4 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                    if (pesquisaProduto)
                    {
                        obs4 = $"{opcao.quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode)}";
                        ClasseDeSuporte.Observações.Add(obs4);
                    }
                    else
                    {
                        string precoProduto = $"- {opcao.TotalPrice.Value.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs4 = $"{opcao.name} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs4);
                    }

                    continue;
                }

                if (obs5 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                    if (pesquisaProduto)
                    {
                        obs5 = $"{opcao.quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode)}";
                        ClasseDeSuporte.Observações.Add(obs5);
                    }
                    else
                    {
                        string precoProduto = $"- {opcao.TotalPrice.Value.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs5 = $"{opcao.name} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs5);
                    }

                    continue;
                }

                if (obs6 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                    if (pesquisaProduto)
                    {
                        obs6 = $"{opcao.quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode)}";
                        ClasseDeSuporte.Observações.Add(obs6);
                    }
                    else
                    {
                        string precoProduto = $"- {opcao.TotalPrice.Value.ToString("c")}";
                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs6 = $"{opcao.name} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs6);
                    }

                    continue;
                }

                if (obs7 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                    if (pesquisaProduto)
                    {
                        obs7 = $"{opcao.quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode)}";
                        ClasseDeSuporte.Observações.Add(obs7);
                    }
                    else
                    {
                        string precoProduto = $"- {opcao.TotalPrice.Value.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs7 = $"{opcao.name} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs7);
                    }

                    continue;
                }

                if (obs8 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                    if (pesquisaProduto)
                    {
                        obs8 = $"{opcao.quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode)}";
                        ClasseDeSuporte.Observações.Add(obs8);
                    }
                    else
                    {
                        string precoProduto = $"- {opcao.TotalPrice.Value.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs8 = $"{opcao.name} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs8);
                    }

                    continue;
                }

                if (obs9 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                    if (pesquisaProduto)
                    {
                        obs9 = $"{opcao.quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode)}";
                        ClasseDeSuporte.Observações.Add(obs9);
                    }
                    else
                    {
                        string precoProduto = $"- {opcao.TotalPrice.Value.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs9 = $"{opcao.name} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs9);
                    }

                    continue;
                }

                if (obs10 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                    if (pesquisaProduto)
                    {
                        obs10 = $"{opcao.quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode)}";
                        ClasseDeSuporte.Observações.Add(obs10);
                    }
                    else
                    {
                        string precoProduto = $"- {opcao.TotalPrice.Value.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs10 = $"{opcao.name} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs10);
                    }

                    continue;
                }

                if (obs11 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                    if (pesquisaProduto)
                    {
                        obs11 = $"{opcao.quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode)}";
                        ClasseDeSuporte.Observações.Add(obs11);
                    }
                    else
                    {
                        string precoProduto = $"- {opcao.TotalPrice.Value.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs11 = $"{opcao.name} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs11);
                    }

                    continue;
                }

                if (obs12 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                    if (pesquisaProduto)
                    {
                        obs12 = $"{opcao.quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode)}";
                        ClasseDeSuporte.Observações.Add(obs12);
                    }
                    else
                    {
                        string precoProduto = $"- {opcao.TotalPrice.Value.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs12 = $"{opcao.name} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs12);
                    }

                    continue;
                }

                if (obs13 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                    if (pesquisaProduto)
                    {
                        obs13 = $"{opcao.quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode)}";
                        ClasseDeSuporte.Observações.Add(obs13);
                    }
                    else
                    {
                        string precoProduto = $"- {opcao.TotalPrice.Value.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs13 = $"{opcao.name} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs13);
                    }

                    continue;
                }

                if (obs14 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                    if (pesquisaProduto)
                    {
                        obs14 = $"{opcao.quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode)}";
                        ClasseDeSuporte.Observações.Add(obs14);
                    }
                    else
                    {
                        string precoProduto = $"- {opcao.TotalPrice.Value.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs14 = $"{opcao.name} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs14);
                    }

                    continue;
                }


            }

            ClasseDeSuporte.Tamanho = "U";

            ClasseDeSuporte.ExternalCode1 = externalCode1;
            ClasseDeSuporte.ExternalCode2 = externalCode2;
            ClasseDeSuporte.ExternalCode3 = externalCode3;

            ClasseDeSuporte.Obs1 = obs1;
            ClasseDeSuporte.Obs2 = obs2;
            ClasseDeSuporte.Obs3 = obs3;
            ClasseDeSuporte.Obs4 = obs4;
            ClasseDeSuporte.Obs5 = obs5;
            ClasseDeSuporte.Obs6 = obs6;
            ClasseDeSuporte.Obs7 = obs7;
            ClasseDeSuporte.Obs8 = obs8;
            ClasseDeSuporte.Obs9 = obs9;
            ClasseDeSuporte.Obs10 = obs10;
            ClasseDeSuporte.Obs11 = obs11;
            ClasseDeSuporte.Obs12 = obs12;
            ClasseDeSuporte.Obs13 = obs13;
            ClasseDeSuporte.Obs14 = obs14;

            return ClasseDeSuporte;

        }
    }

    public static ClsDeSuporteParaImpressaoDosItens DefineCaracteristicasDoItem(Items item, bool comanda = false)
    {
        string? NomeProduto = "";
        ClsDeSuporteParaImpressaoDosItens ClasseDeSuporte = new ClsDeSuporteParaImpressaoDosItens();

        bool ePizza = item.externalCode == "G" || item.externalCode == "M" || item.externalCode == "P" ? true : false;

        if (ePizza)
        {
            string obs = item.observations == null || item.observations == "" ? " " : item.observations.ToString();
            string externalCode = " ";

            string? ePizza1 = null;
            string? ePizza2 = null;
            string? ePizza3 = null;

            string? obs1 = " ";
            string? obs2 = " ";
            string? obs3 = " ";
            string? obs4 = " ";
            string? obs5 = " ";
            string? obs6 = " ";
            string? obs7 = " ";
            string? obs8 = " ";
            string? obs9 = " ";
            string? obs10 = " ";
            string? obs11 = " ";
            string? obs12 = " ";
            string? obs13 = " ";
            string? obs14 = " ";

            foreach (var option in item.options)
            {
                if (!option.externalCode.Contains("m") && ePizza1 == null)
                {
                    ePizza1 = option.externalCode == "" ? " " : option.externalCode;
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(option.externalCode);

                    if (pesquisaProduto)
                    {
                        NomeProduto += ClsDeIntegracaoSys.NomeProdutoCardapio(option.externalCode);
                    }
                    else
                    {
                        NomeProduto += option.name;
                    }
                    continue;
                }

                if (!option.externalCode.Contains("m") && ePizza2 == null)
                {
                    ePizza2 = option.externalCode == "" ? " " : option.externalCode;
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(option.externalCode);

                    if (pesquisaProduto)
                    {
                        NomeProduto += " / " + ClsDeIntegracaoSys.NomeProdutoCardapio(option.externalCode);
                    }
                    else
                    {
                        NomeProduto += " / " + option.name;
                    }
                    continue;
                }

                if (!option.externalCode.Contains("m") && ePizza3 == null)
                {
                    ePizza3 = option.externalCode == "" ? " " : option.externalCode;
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(option.externalCode);

                    if (pesquisaProduto)
                    {
                        NomeProduto += " / " + ClsDeIntegracaoSys.NomeProdutoCardapio(option.externalCode);
                    }
                    else
                    {
                        NomeProduto += " / " + option.name;
                    }
                    continue;
                }

            }


            foreach (var opcao in item.options)
            {
                if (obs1 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                    if (pesquisaProduto && opcao.externalCode.Contains("m"))
                    {
                        obs1 = $"{opcao.quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode)}";
                        ClasseDeSuporte.Observações.Add(obs1);
                    }
                    else if (opcao.externalCode.Contains("m"))
                    {
                        string precoProduto = $"- {opcao.price.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs1 = $"{opcao.quantity}X  {opcao.name} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs1);
                    }

                    continue;
                }

                if (obs2 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                    if (pesquisaProduto && opcao.externalCode.Contains("m"))
                    {
                        obs2 = $"{opcao.quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode)}";
                        ClasseDeSuporte.Observações.Add(obs2);
                    }
                    else if (opcao.externalCode.Contains("m"))
                    {
                        string precoProduto = $"- {opcao.price.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs2 = $"{opcao.quantity}X  {opcao.name} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs2);

                    }

                    continue;
                }

                if (obs3 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                    if (pesquisaProduto && opcao.externalCode.Contains("m"))
                    {
                        obs3 = $"{opcao.quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode)}";
                        ClasseDeSuporte.Observações.Add(obs3);

                    }
                    else if (opcao.externalCode.Contains("m"))
                    {
                        string precoProduto = $"- {opcao.price.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs3 = $"{opcao.quantity}X  {opcao.name} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs3);

                    }

                    continue;
                }

                if (obs4 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                    if (pesquisaProduto && opcao.externalCode.Contains("m"))
                    {
                        obs4 = $"{opcao.quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode)}";
                        ClasseDeSuporte.Observações.Add(obs4);

                    }
                    else if (opcao.externalCode.Contains("m"))
                    {
                        string precoProduto = $"- {opcao.price.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs4 = $"{opcao.quantity}X  {opcao.name} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs4);

                    }

                    continue;
                }

                if (obs5 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                    if (pesquisaProduto && opcao.externalCode.Contains("m"))
                    {
                        obs5 = $"{opcao.quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode)}";
                        ClasseDeSuporte.Observações.Add(obs5);

                    }
                    else if (opcao.externalCode.Contains("m"))
                    {
                        string precoProduto = $"- {opcao.price.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs5 = $"{opcao.quantity}X  {opcao.name} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs5);

                    }

                    continue;
                }

                if (obs6 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                    if (pesquisaProduto && opcao.externalCode.Contains("m"))
                    {
                        obs6 = $"{opcao.quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode)}";
                        ClasseDeSuporte.Observações.Add(obs6);

                    }
                    else if (opcao.externalCode.Contains("m"))
                    {
                        string precoProduto = $"- {opcao.price.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs6 = $"{opcao.quantity}X  {opcao.name} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs6);

                    }

                    continue;
                }

                if (obs7 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                    if (pesquisaProduto && opcao.externalCode.Contains("m"))
                    {
                        obs7 = $"{opcao.quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode)}";
                        ClasseDeSuporte.Observações.Add(obs7);

                    }
                    else if (opcao.externalCode.Contains("m"))
                    {
                        string precoProduto = $"- {opcao.price.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs7 = $"{opcao.quantity}X  {opcao.name} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs7);

                    }

                    continue;
                }

                if (obs8 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                    if (pesquisaProduto && opcao.externalCode.Contains("m"))
                    {
                        obs8 = $"{opcao.quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode)}";
                        ClasseDeSuporte.Observações.Add(obs8);

                    }
                    else if (opcao.externalCode.Contains("m"))
                    {
                        string precoProduto = $"- {opcao.price.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs8 = $"{opcao.quantity}X  {opcao.name} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs8);

                    }

                    continue;
                }

                if (obs9 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                    if (pesquisaProduto && opcao.externalCode.Contains("m"))
                    {
                        obs9 = $"{opcao.quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode)}";
                        ClasseDeSuporte.Observações.Add(obs9);

                    }
                    else if (opcao.externalCode.Contains("m"))
                    {
                        string precoProduto = $"- {opcao.price.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs9 = $"{opcao.quantity}X  {opcao.name} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs9);

                    }

                    continue;
                }

                if (obs10 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                    if (pesquisaProduto && opcao.externalCode.Contains("m"))
                    {
                        obs10 = $"{opcao.quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode)}";
                        ClasseDeSuporte.Observações.Add(obs10);

                    }
                    else if (opcao.externalCode.Contains("m"))
                    {
                        string precoProduto = $"- {opcao.price.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs10 = $"{opcao.quantity}X  {opcao.name} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs10);

                    }

                    continue;
                }

                if (obs11 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                    if (pesquisaProduto && opcao.externalCode.Contains("m"))
                    {
                        obs11 = $"{opcao.quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode)}";
                        ClasseDeSuporte.Observações.Add(obs11);

                    }
                    else if (opcao.externalCode.Contains("m"))
                    {
                        string precoProduto = $"- {opcao.price.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs11 = $"{opcao.quantity}X  {opcao.name} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs11);
                    }

                    continue;
                }

                if (obs12 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                    if (pesquisaProduto && opcao.externalCode.Contains("m"))
                    {
                        obs12 = $"{opcao.quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode)}";
                        ClasseDeSuporte.Observações.Add(obs12);

                    }
                    else if (opcao.externalCode.Contains("m"))
                    {
                        string precoProduto = $"- {opcao.price.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs12 = $"{opcao.quantity}X  {opcao.name} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs12);

                    }

                    continue;
                }

                if (obs13 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                    if (pesquisaProduto && opcao.externalCode.Contains("m"))
                    {
                        obs13 = $"{opcao.quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode)}";
                        ClasseDeSuporte.Observações.Add(obs13);

                    }
                    else if (opcao.externalCode.Contains("m"))
                    {
                        string precoProduto = $"- {opcao.price.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs13 = $"{opcao.quantity}X  {opcao.name} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs13);

                    }

                    continue;
                }

                if (obs14 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                    if (pesquisaProduto && opcao.externalCode.Contains("m"))
                    {
                        obs14 = $"{opcao.quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode)}";
                        ClasseDeSuporte.Observações.Add(obs14);

                    }
                    else if (opcao.externalCode.Contains("m"))
                    {
                        string precoProduto = $"- {opcao.price.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs14 = $"{opcao.quantity}X  {opcao.name} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs14);
                    }

                    continue;
                }

            }

            ClasseDeSuporte.NomeProduto = NomeProduto;

            return ClasseDeSuporte;

        }
        else
        {
            string? externalCode = item.externalCode == null || item.externalCode == "" ? " " : item.externalCode;
            string? obs = item.observations == null || item.observations == "" ? " " : item.observations.ToString();

            bool existeProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(externalCode);

            if (existeProduto)
            {
                NomeProduto = ClsDeIntegracaoSys.NomeProdutoCardapio(externalCode);
            }
            else
            {
                NomeProduto = item.name;
            }

            ClasseDeSuporte.NomeProduto = NomeProduto;

            string? obs1 = " ";
            string? obs2 = " ";
            string? obs3 = " ";
            string? obs4 = " ";
            string? obs5 = " ";
            string? obs6 = " ";
            string? obs7 = " ";
            string? obs8 = " ";
            string? obs9 = " ";
            string? obs10 = " ";
            string? obs11 = " ";
            string? obs12 = " ";
            string? obs13 = " ";
            string? obs14 = " ";


            foreach (var opcao in item.options)
            {
                if (obs1 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                    if (pesquisaProduto)
                    {
                        obs1 = $"{opcao.quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode)}";
                        ClasseDeSuporte.Observações.Add(obs1);
                    }
                    else
                    {
                        string precoProduto = $"- {opcao.price.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs1 = $"{opcao.quantity}X  {opcao.name} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs1);
                    }

                    continue;
                }

                if (obs2 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                    if (pesquisaProduto)
                    {
                        obs2 = $"{opcao.quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode)}";
                        ClasseDeSuporte.Observações.Add(obs2);
                    }
                    else
                    {
                        string precoProduto = $"- {opcao.price.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs2 = $"{opcao.quantity}X  {opcao.name} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs2);
                    }

                    continue;
                }

                if (obs3 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                    if (pesquisaProduto)
                    {
                        obs3 = $"{opcao.quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode)}";
                        ClasseDeSuporte.Observações.Add(obs3);
                    }
                    else
                    {
                        string precoProduto = $"- {opcao.price.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs3 = $"{opcao.quantity}X  {opcao.name} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs3);
                    }

                    continue;
                }

                if (obs4 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                    if (pesquisaProduto)
                    {
                        obs4 = $"{opcao.quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode)}";
                        ClasseDeSuporte.Observações.Add(obs4);
                    }
                    else
                    {
                        string precoProduto = $"- {opcao.price.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs4 = $"{opcao.quantity}X  {opcao.name} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs4);
                    }

                    continue;
                }

                if (obs5 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                    if (pesquisaProduto)
                    {
                        obs5 = $"{opcao.quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode)}";
                        ClasseDeSuporte.Observações.Add(obs5);
                    }
                    else
                    {
                        string precoProduto = $"- {opcao.price.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs5 = $"{opcao.quantity}X  {opcao.name} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs5);
                    }

                    continue;
                }

                if (obs6 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                    if (pesquisaProduto)
                    {
                        obs6 = $"{opcao.quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode)}";
                        ClasseDeSuporte.Observações.Add(obs6);
                    }
                    else
                    {
                        string precoProduto = $"- {opcao.price.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs6 = $"{opcao.quantity}X  {opcao.name} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs6);
                    }

                    continue;
                }

                if (obs7 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                    if (pesquisaProduto)
                    {
                        obs7 = $"{opcao.quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode)}";
                        ClasseDeSuporte.Observações.Add(obs7);
                    }
                    else
                    {
                        string precoProduto = $"- {opcao.price.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs7 = $"{opcao.quantity}X  {opcao.name} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs7);
                    }

                    continue;
                }

                if (obs8 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                    if (pesquisaProduto)
                    {
                        obs8 = $"{opcao.quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode)}";
                        ClasseDeSuporte.Observações.Add(obs8);
                    }
                    else
                    {
                        string precoProduto = $"- {opcao.price.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs8 = $"{opcao.quantity}X  {opcao.name} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs8);
                    }

                    continue;
                }

                if (obs9 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                    if (pesquisaProduto)
                    {
                        obs9 = $"{opcao.quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode)}";
                        ClasseDeSuporte.Observações.Add(obs9);
                    }
                    else
                    {
                        string precoProduto = $"- {opcao.price.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs9 = $"{opcao.quantity}X  {opcao.name} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs9);
                    }

                    continue;
                }

                if (obs10 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                    if (pesquisaProduto)
                    {
                        obs10 = $"{opcao.quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode)}";
                        ClasseDeSuporte.Observações.Add(obs10);
                    }
                    else
                    {
                        string precoProduto = $"- {opcao.price.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs10 = $"{opcao.quantity}X  {opcao.name} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs10);
                    }

                    continue;
                }

                if (obs11 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                    if (pesquisaProduto)
                    {
                        obs11 = $"{opcao.quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode)}";
                        ClasseDeSuporte.Observações.Add(obs11);
                    }
                    else
                    {
                        string precoProduto = $"- {opcao.price.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs11 = $"{opcao.quantity}X  {opcao.name} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs11);
                    }

                    continue;
                }

                if (obs12 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                    if (pesquisaProduto)
                    {
                        obs12 = $"{opcao.quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode)}";
                        ClasseDeSuporte.Observações.Add(obs12);
                    }
                    else
                    {
                        string precoProduto = $"- {opcao.price.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs12 = $"{opcao.quantity}X  {opcao.name} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs12);
                    }

                    continue;
                }

                if (obs13 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                    if (pesquisaProduto)
                    {
                        obs13 = $"{opcao.quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode)}";
                        ClasseDeSuporte.Observações.Add(obs13);
                    }
                    else
                    {
                        string precoProduto = $"- {opcao.price.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs13 = $"{opcao.quantity}X  {opcao.name} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs13);
                    }

                    continue;
                }

                if (obs14 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.externalCode);

                    if (pesquisaProduto)
                    {
                        obs14 = $"{opcao.quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.externalCode)}";
                        ClasseDeSuporte.Observações.Add(obs14);
                    }
                    else
                    {
                        string precoProduto = $"- {opcao.price.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs14 = $"{opcao.quantity}X  {opcao.name} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs14);
                    }

                    continue;
                }


            }

            return ClasseDeSuporte;

        }
    }

    public static List<string> DefineNomeImpressoraPorProduto(string codCardapio)
    {
        List<string> locaisImp = new List<string>();
        try
        {
            string banco = CaminhoBaseSysMenu;//@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\gui-c\OneDrive\Área de Trabalho\SysIntegrador\CONTAS.mdb";
            using ApplicationDbContext dbPostgres = new ApplicationDbContext();
            ParametrosDoSistema? opcSistema = dbPostgres.parametrosdosistema.ToList().FirstOrDefault();

            string? caminhoBancoAccess = opcSistema.CaminhodoBanco.Replace("CONTAS", "CADASTROS");

            using (OleDbConnection connection = new OleDbConnection(caminhoBancoAccess))
            {
                connection.Open();

                string SqlSelectIntoCadastros = "SELECT * FROM Cardapio WHERE CODIGO = @CODIGO";


                using (OleDbCommand selectCommand = new OleDbCommand(SqlSelectIntoCadastros, connection))
                {
                    selectCommand.Parameters.AddWithValue("@CODIGO", codCardapio);

                    // Executar a consulta SELECT
                    using (OleDbDataReader reader = selectCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            locaisImp.Add(reader["IMPCOMANDA"].ToString());
                            locaisImp.Add(reader["IMPCOMANDA2"].ToString());
                        }
                    }
                }

                if (locaisImp.Count() == 0)
                {
                    locaisImp.Add("Cz1");
                    locaisImp.Add("Não");
                }

                return locaisImp;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString(), "Erro ao definir local de impresão.");
        }
        return locaisImp;
    }


    public static bool VerificaSeExisteProdutoComExternalCode(string codCardapio)
    {
        bool existeProduto = false;
        try
        {
            if (codCardapio != null)
            {
                string banco = CaminhoBaseSysMenu;//@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\gui-c\OneDrive\Área de Trabalho\SysIntegrador\CONTAS.mdb";
                using ApplicationDbContext dbPostgres = new ApplicationDbContext();
                ParametrosDoSistema? opcSistema = dbPostgres.parametrosdosistema.ToList().FirstOrDefault();

                string? caminhoBancoAccess = opcSistema.CaminhodoBanco.Replace("CONTAS", "CADASTROS");

                using (OleDbConnection connection = new OleDbConnection(caminhoBancoAccess))
                {
                    connection.Open();

                    string SqlSelectIntoCadastros = "SELECT * FROM Cardapio WHERE CODIGO = @CODIGO";

                    List<string> locaisImp = new List<string>();

                    using (OleDbCommand selectCommand = new OleDbCommand(SqlSelectIntoCadastros, connection))
                    {
                        selectCommand.Parameters.AddWithValue("@CODIGO", codCardapio);

                        // Executar a consulta SELECT
                        using (OleDbDataReader reader = selectCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                locaisImp.Add(reader["IMPCOMANDA"].ToString());
                                locaisImp.Add(reader["IMPCOMANDA2"].ToString());
                            }
                        }
                    }

                    if (locaisImp.Count() != 0)
                    {
                        existeProduto = true;
                    }

                    return existeProduto;
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString(), "Erro ao definir local de impresão.");
        }
        return existeProduto;
    }


    public static async Task<bool> VerificaCaixaAberto()
    {
        bool CaixaAberto = false;
        try
        {

            string banco = CaminhoBaseSysMenu;//@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\gui-c\OneDrive\Área de Trabalho\SysIntegrador\CONTAS.mdb";
            using ApplicationDbContext dbPostgres = new ApplicationDbContext();
            ParametrosDoSistema? opcSistema = dbPostgres.parametrosdosistema.ToList().FirstOrDefault();

            string? caminhoBancoAccess = opcSistema.CaminhodoBanco;

            using (OleDbConnection connection = new OleDbConnection(caminhoBancoAccess))
            {
                connection.Open();

                string sqlSelect = "SELECT COUNT(*) FROM Caixa ";


                using (OleDbCommand selectCommand = new OleDbCommand(sqlSelect, connection))
                {
                    // Executar a consulta SELECT
                    int count = (int)selectCommand.ExecuteScalar();
                    CaixaAberto = count > 0;
                }

                return CaixaAberto;
            }
        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
            MessageBox.Show(ex.Message, "Ops");
        }
        return CaixaAberto;
    }


    public static async void ExcluiPedidoCasoCancelado(string orderId, bool mesa = false)
    {
        try
        {
            ApplicationDbContext dbPostgres = new ApplicationDbContext();
            var pedido = dbPostgres.parametrosdopedido.Where(x => x.Id == orderId).ToList().FirstOrDefault();
            int NumConta = pedido.Conta;
            string banco = CaminhoBaseSysMenu;

            if (mesa)
            {
                using (OleDbConnection connection = new OleDbConnection(banco))
                {
                    ParametrosDoPedido? PedidoDB = dbPostgres.parametrosdopedido.Where(x => x.Id == orderId).FirstOrDefault();
                    PedidoOnPedido? Pedido = JsonConvert.DeserializeObject<PedidoOnPedido>(PedidoDB.Json);

                    OnPedido OnPedido = new OnPedido(new MeuContexto());

                    string numMesa = await OnPedido.RetiraNumeroDeMesa(Pedido.Return.Indoor.Place);

                    connection.Open();

                    string deleteCommandText = "DELETE FROM Contas WHERE MESA = @MESA";
                    OleDbCommand deleteCommand = new OleDbCommand(deleteCommandText, connection);

                    deleteCommand.Parameters.AddWithValue("@MESA", numMesa);

                    deleteCommand.ExecuteNonQuery();

                }//faz o delete da pagCartao
            }


            using (OleDbConnection connection = new OleDbConnection(banco))
            {
                connection.Open();

                string deleteCommandText = "DELETE FROM Sequencia WHERE CONTA = @NUMCONTA";
                OleDbCommand deleteCommand = new OleDbCommand(deleteCommandText, connection);

                deleteCommand.Parameters.AddWithValue("@NUMCONTA", NumConta);

                deleteCommand.ExecuteNonQuery();

            }///faz o delete da sequencia

            using (OleDbConnection connection = new OleDbConnection(banco))
            {

                connection.Open();

                string deleteCommandText = "DELETE FROM Contas WHERE CONTA = @NUMCONTA";
                OleDbCommand deleteCommand = new OleDbCommand(deleteCommandText, connection);

                deleteCommand.Parameters.AddWithValue("@NUMCONTA", NumConta);

                deleteCommand.ExecuteNonQuery();

            }//faz o delete da contas

            using (OleDbConnection connection = new OleDbConnection(banco))
            {

                connection.Open();

                string deleteCommandText = "DELETE FROM PagCartao WHERE CONTA = @NUMCONTA";
                OleDbCommand deleteCommand = new OleDbCommand(deleteCommandText, connection);

                deleteCommand.Parameters.AddWithValue("@NUMCONTA", NumConta);

                deleteCommand.ExecuteNonQuery();

            }//faz o delete da pagCartao

        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Ops");
        }
    }


    public static ClsDeSuporteParaImpressaoDosItens DefineCaracteristicasDoItemDelMatch(items item, bool comanda = false)
    {
        string? NomeProduto = "";
        ClsDeSuporteParaImpressaoDosItens ClasseDeSuporte = new ClsDeSuporteParaImpressaoDosItens();

        bool ePizza = item.ExternalCode == "G" || item.ExternalCode == "M" || item.ExternalCode == "P" ? true : false;

        string? ObsDoItem = " ";

        if (item.Observations != null && item.Observations != "")
        {
            if (item.Observations.Length > 80)
            {
                ObsDoItem = item.Observations.Substring(0, 80);
            }
            else
            {
                ObsDoItem = item.Observations;
            }
        }

        ClasseDeSuporte.ObsDoItem = ObsDoItem;

        if (ePizza)
        {
            string obs = item.ExternalCode == null || item.ExternalCode == "" ? " " : item.ExternalCode.ToString();
            string externalCode1 = " ";
            string externalCode2 = " ";
            string externalCode3 = " ";

            string? ePizza1 = null;
            string? ePizza2 = null;
            string? ePizza3 = null;

            string? obs1 = " ";
            string? obs2 = " ";
            string? obs3 = " ";
            string? obs4 = " ";
            string? obs5 = " ";
            string? obs6 = " ";
            string? obs7 = " ";
            string? obs8 = " ";
            string? obs9 = " ";
            string? obs10 = " ";
            string? obs11 = " ";
            string? obs12 = " ";
            string? obs13 = " ";
            string? obs14 = " ";


            foreach (var option in item.SubItems)
            {
                if (!option.ExternalCode.Contains("m") && ePizza1 == null)
                {
                    ePizza1 = option.ExternalCode == "" ? " " : option.ExternalCode;
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(option.ExternalCode);

                    if (pesquisaProduto)
                    {
                        NomeProduto = $"{item.Quantity}X "  + ClsDeIntegracaoSys.NomeProdutoCardapio(option.ExternalCode);
                        externalCode1 = option.ExternalCode;
                    }
                    else
                    {
                        NomeProduto = $"{item.Quantity}X " + option.Name;
                    }
                    continue;
                }

                if (!option.ExternalCode.Contains("m") && ePizza2 == null)
                {
                    ePizza2 = option.ExternalCode == "" ? " " : option.ExternalCode;
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(option.ExternalCode);

                    if (pesquisaProduto)
                    {
                        NomeProduto += " / " + ClsDeIntegracaoSys.NomeProdutoCardapio(option.ExternalCode);
                        externalCode2 = option.ExternalCode;
                    }
                    else
                    {
                        NomeProduto += " / " + option.Name;
                    }
                    continue;
                }

                if (!option.ExternalCode.Contains("m") && ePizza3 == null)
                {
                    ePizza3 = option.ExternalCode == "" ? " " : option.ExternalCode;
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(option.ExternalCode);

                    if (pesquisaProduto)
                    {
                        NomeProduto += " / " + ClsDeIntegracaoSys.NomeProdutoCardapio(option.ExternalCode);
                        externalCode3 = option.ExternalCode;
                    }
                    else
                    {
                        NomeProduto += " / " + option.Name;
                    }
                    continue;
                }

            }

            ClasseDeSuporte.Tamanho = item.ExternalCode;
            ClasseDeSuporte.ExternalCode1 = externalCode1;
            ClasseDeSuporte.ExternalCode2 = externalCode2;
            ClasseDeSuporte.ExternalCode3 = externalCode3;


            foreach (var opcao in item.SubItems)
            {
                if (obs1 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.ExternalCode);

                    if (pesquisaProduto && opcao.ExternalCode.Contains("m"))
                    {
                        obs1 = $"{opcao.Quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.ExternalCode)}";
                        ClasseDeSuporte.Observações.Add(obs1);
                    }
                    else if (opcao.ExternalCode.Contains("m"))
                    {
                        string precoProduto = $"- {opcao.Price.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs1 = $"{opcao.Quantity}X  {opcao.Name} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs1);
                    }

                    continue;
                }

                if (obs2 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.ExternalCode);

                    if (pesquisaProduto && opcao.ExternalCode.Contains("m"))
                    {
                        obs2 = $"{opcao.Quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.ExternalCode)}";
                        ClasseDeSuporte.Observações.Add(obs2);
                    }
                    else if (opcao.ExternalCode.Contains("m"))
                    {
                        string precoProduto = $"- {opcao.Price.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs2 = $"{opcao.Quantity}X  {opcao.Name} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs2);

                    }

                    continue;
                }

                if (obs3 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.ExternalCode);

                    if (pesquisaProduto && opcao.ExternalCode.Contains("m"))
                    {
                        obs3 = $"{opcao.Quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.ExternalCode)}";
                        ClasseDeSuporte.Observações.Add(obs3);

                    }
                    else if (opcao.ExternalCode.Contains("m"))
                    {
                        string precoProduto = $"- {opcao.Price.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs3 = $"{opcao.Quantity}X  {opcao.Name} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs3);

                    }

                    continue;
                }

                if (obs4 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.ExternalCode);

                    if (pesquisaProduto && opcao.ExternalCode.Contains("m"))
                    {
                        obs4 = $"{opcao.Quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.ExternalCode)}";
                        ClasseDeSuporte.Observações.Add(obs4);

                    }
                    else if (opcao.ExternalCode.Contains("m"))
                    {
                        string precoProduto = $"- {opcao.Price.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs4 = $"{opcao.Quantity}X  {opcao.Name} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs4);

                    }

                    continue;
                }

                if (obs5 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.ExternalCode);

                    if (pesquisaProduto && opcao.ExternalCode.Contains("m"))
                    {
                        obs5 = $"{opcao.Quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.ExternalCode)}";
                        ClasseDeSuporte.Observações.Add(obs5);

                    }
                    else if (opcao.ExternalCode.Contains("m"))
                    {
                        string precoProduto = $"- {opcao.Price.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs5 = $"{opcao.Quantity}X  {opcao.Name} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs5);

                    }

                    continue;
                }

                if (obs6 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.ExternalCode);

                    if (pesquisaProduto && opcao.ExternalCode.Contains("m"))
                    {
                        obs6 = $"{opcao.Quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.ExternalCode)}";
                        ClasseDeSuporte.Observações.Add(obs6);

                    }
                    else if (opcao.ExternalCode.Contains("m"))
                    {
                        string precoProduto = $"- {opcao.Price.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs6 = $"{opcao.Quantity}X  {opcao.Name} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs6);

                    }

                    continue;
                }

                if (obs7 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.ExternalCode);

                    if (pesquisaProduto && opcao.ExternalCode.Contains("m"))
                    {
                        obs7 = $"{opcao.Quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.ExternalCode)}";
                        ClasseDeSuporte.Observações.Add(obs7);

                    }
                    else if (opcao.ExternalCode.Contains("m"))
                    {
                        string precoProduto = $"- {opcao.Price.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs7 = $"{opcao.Quantity}X  {opcao.Name} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs7);

                    }

                    continue;
                }

                if (obs8 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.ExternalCode);

                    if (pesquisaProduto && opcao.ExternalCode.Contains("m"))
                    {
                        obs8 = $"{opcao.Quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.ExternalCode)}";
                        ClasseDeSuporte.Observações.Add(obs8);

                    }
                    else if (opcao.ExternalCode.Contains("m"))
                    {
                        string precoProduto = $"- {opcao.Price.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs8 = $"{opcao.Quantity}X  {opcao.Name} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs8);

                    }

                    continue;
                }

                if (obs9 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.ExternalCode);

                    if (pesquisaProduto && opcao.ExternalCode.Contains("m"))
                    {
                        obs8 = $"{opcao.Quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.ExternalCode)}";
                        ClasseDeSuporte.Observações.Add(obs9);

                    }
                    else if (opcao.ExternalCode.Contains("m"))
                    {
                        string precoProduto = $"- {opcao.Price.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs9 = $"{opcao.Quantity}X  {opcao.Name} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs9);

                    }

                    continue;
                }

                if (obs10 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.ExternalCode);

                    if (pesquisaProduto && opcao.ExternalCode.Contains("m"))
                    {
                        obs10 = $"{opcao.ExternalCode}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.ExternalCode)}";
                        ClasseDeSuporte.Observações.Add(obs10);

                    }
                    else if (opcao.ExternalCode.Contains("m"))
                    {
                        string precoProduto = $"- {opcao.Price.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs10 = $"{opcao.Quantity}X  {opcao.Name} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs10);

                    }

                    continue;
                }

                if (obs11 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.ExternalCode);

                    if (pesquisaProduto && opcao.ExternalCode.Contains("m"))
                    {
                        obs11 = $"{opcao.Quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.ExternalCode)}";
                        ClasseDeSuporte.Observações.Add(obs11);

                    }
                    else if (opcao.ExternalCode.Contains("m"))
                    {
                        string precoProduto = $"- {opcao.Price.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs11 = $"{opcao.Quantity}X  {opcao.Name} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs11);
                    }

                    continue;
                }

                if (obs12 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.ExternalCode);

                    if (pesquisaProduto && opcao.ExternalCode.Contains("m"))
                    {
                        obs12 = $"{opcao.Quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.ExternalCode)}";
                        ClasseDeSuporte.Observações.Add(obs12);

                    }
                    else if (opcao.ExternalCode.Contains("m"))
                    {
                        string precoProduto = $"- {opcao.Price.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs12 = $"{opcao.Quantity}X  {opcao.Name} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs12);

                    }

                    continue;
                }

                if (obs13 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.ExternalCode);

                    if (pesquisaProduto && opcao.ExternalCode.Contains("m"))
                    {
                        obs13 = $"{opcao.Quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.ExternalCode)}";
                        ClasseDeSuporte.Observações.Add(obs13);

                    }
                    else if (opcao.ExternalCode.Contains("m"))
                    {
                        string precoProduto = $"- {opcao.Price.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs13 = $"{opcao.Quantity}X  {opcao.Name} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs13);

                    }

                    continue;
                }

                if (obs14 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.ExternalCode);

                    if (pesquisaProduto && opcao.ExternalCode.Contains("m"))
                    {
                        obs14 = $"{opcao.Quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.ExternalCode)}";
                        ClasseDeSuporte.Observações.Add(obs14);

                    }
                    else if (opcao.ExternalCode.Contains("m"))
                    {
                        string precoProduto = $"- {opcao.Price.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs14 = $"{opcao.Quantity}X  {opcao.Name} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs14);
                    }

                    continue;
                }

            }

            ClasseDeSuporte.NomeProduto = NomeProduto;
            ClasseDeSuporte.Obs1 = obs1;
            ClasseDeSuporte.Obs2 = obs2;
            ClasseDeSuporte.Obs3 = obs3;
            ClasseDeSuporte.Obs4 = obs4;
            ClasseDeSuporte.Obs5 = obs5;
            ClasseDeSuporte.Obs6 = obs6;
            ClasseDeSuporte.Obs7 = obs7;
            ClasseDeSuporte.Obs8 = obs8;
            ClasseDeSuporte.Obs9 = obs9;
            ClasseDeSuporte.Obs10 = obs10;
            ClasseDeSuporte.Obs11 = obs11;
            ClasseDeSuporte.Obs12 = obs12;
            ClasseDeSuporte.Obs13 = obs13;
            ClasseDeSuporte.Obs14 = obs14;

            return ClasseDeSuporte;

        }
        else
        {
            string? externalCode = item.ExternalCode == null || item.ExternalCode == "" ? " " : item.ExternalCode;
            string? obs = item.Observations == null || item.Observations == "" ? " " : item.Observations.ToString();

            string? externalCode1 = " ";
            string? externalCode2 = " ";
            string? externalCode3 = " ";

            bool existeProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(externalCode);

            if (existeProduto)
            {
                NomeProduto =  $"{item.Quantity}X " + ClsDeIntegracaoSys.NomeProdutoCardapio(externalCode);
                externalCode1 = externalCode;
            }
            else
            {
                NomeProduto = $"{item.Quantity}X " + item.Name;
            }

            ClasseDeSuporte.NomeProduto = NomeProduto;

            string? obs1 = " ";
            string? obs2 = " ";
            string? obs3 = " ";
            string? obs4 = " ";
            string? obs5 = " ";
            string? obs6 = " ";
            string? obs7 = " ";
            string? obs8 = " ";
            string? obs9 = " ";
            string? obs10 = " ";
            string? obs11 = " ";
            string? obs12 = " ";
            string? obs13 = " ";
            string? obs14 = " ";


            foreach (var opcao in item.SubItems)
            {
                if (obs1 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.ExternalCode);

                    if (pesquisaProduto)
                    {
                        obs1 = $"{opcao.Quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.ExternalCode)}";
                        ClasseDeSuporte.Observações.Add(obs1);
                    }
                    else
                    {
                        string precoProduto = $"- {opcao.Price.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs1 = $"{opcao.Quantity}X  {opcao.Name} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs1);
                    }

                    continue;
                }

                if (obs2 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.ExternalCode);

                    if (pesquisaProduto)
                    {
                        obs2 = $"{opcao.Quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.ExternalCode)}";
                        ClasseDeSuporte.Observações.Add(obs2);
                    }
                    else
                    {
                        string precoProduto = $"- {opcao.Price.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs2 = $"{opcao.Quantity}X  {opcao.Name} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs2);
                    }

                    continue;
                }

                if (obs3 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.ExternalCode);

                    if (pesquisaProduto)
                    {
                        obs3 = $"{opcao.Quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.ExternalCode)}";
                        ClasseDeSuporte.Observações.Add(obs3);
                    }
                    else
                    {
                        string precoProduto = $"- {opcao.Price.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs3 = $"{opcao.Quantity}X  {opcao.Name} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs3);
                    }

                    continue;
                }

                if (obs4 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.ExternalCode);

                    if (pesquisaProduto)
                    {
                        obs4 = $"{opcao.Quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.ExternalCode)}";
                        ClasseDeSuporte.Observações.Add(obs4);
                    }
                    else
                    {
                        string precoProduto = $"- {opcao.Price.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs4 = $"{opcao.Quantity}X  {opcao.Name} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs4);
                    }

                    continue;
                }

                if (obs5 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.ExternalCode);

                    if (pesquisaProduto)
                    {
                        obs5 = $"{opcao.Quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.ExternalCode)}";
                        ClasseDeSuporte.Observações.Add(obs5);
                    }
                    else
                    {
                        string precoProduto = $"- {opcao.Price.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs5 = $"{opcao.Quantity}X  {opcao.Name} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs5);
                    }

                    continue;
                }

                if (obs6 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.ExternalCode);

                    if (pesquisaProduto)
                    {
                        obs6 = $"{opcao.Quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.ExternalCode)}";
                        ClasseDeSuporte.Observações.Add(obs6);
                    }
                    else
                    {
                        string precoProduto = $"- {opcao.Price.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs6 = $"{opcao.Quantity}X  {opcao.Name} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs6);
                    }

                    continue;
                }

                if (obs7 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.ExternalCode);

                    if (pesquisaProduto)
                    {
                        obs7 = $"{opcao.Quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.ExternalCode)}";
                        ClasseDeSuporte.Observações.Add(obs7);
                    }
                    else
                    {
                        string precoProduto = $"- {opcao.Price.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs7 = $"{opcao.Quantity}X  {opcao.Name} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs7);
                    }

                    continue;
                }

                if (obs8 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.ExternalCode);

                    if (pesquisaProduto)
                    {
                        obs8 = $"{opcao.Quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.ExternalCode)}";
                        ClasseDeSuporte.Observações.Add(obs8);
                    }
                    else
                    {
                        string precoProduto = $"- {opcao.Price.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs8 = $"{opcao.Quantity}X  {opcao.Name} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs8);
                    }

                    continue;
                }

                if (obs9 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.ExternalCode);

                    if (pesquisaProduto)
                    {
                        obs9 = $"{opcao.Quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.ExternalCode)}";
                        ClasseDeSuporte.Observações.Add(obs9);
                    }
                    else
                    {
                        string precoProduto = $"- {opcao.Price.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs9 = $"{opcao.Quantity}X  {opcao.Name} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs9);
                    }

                    continue;
                }

                if (obs10 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.ExternalCode);

                    if (pesquisaProduto)
                    {
                        obs10 = $"{opcao.Quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.ExternalCode)}";
                        ClasseDeSuporte.Observações.Add(obs10);
                    }
                    else
                    {
                        string precoProduto = $"- {opcao.Price.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs10 = $"{opcao.Quantity}X  {opcao.Name} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs10);
                    }

                    continue;
                }

                if (obs11 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.ExternalCode);

                    if (pesquisaProduto)
                    {
                        obs11 = $"{opcao.Quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.ExternalCode)}";
                        ClasseDeSuporte.Observações.Add(obs11);
                    }
                    else
                    {
                        string precoProduto = $"- {opcao.Price.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs11 = $"{opcao.Quantity}X  {opcao.Name} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs11);
                    }

                    continue;
                }

                if (obs12 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.ExternalCode);

                    if (pesquisaProduto)
                    {
                        obs12 = $"{opcao.Quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.ExternalCode)}";
                        ClasseDeSuporte.Observações.Add(obs12);
                    }
                    else
                    {
                        string precoProduto = $"- {opcao.Price.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs12 = $"{opcao.Quantity}X  {opcao.Name} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs12);
                    }

                    continue;
                }

                if (obs13 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.ExternalCode);

                    if (pesquisaProduto)
                    {
                        obs13 = $"{opcao.Quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.ExternalCode)}";
                        ClasseDeSuporte.Observações.Add(obs13);
                    }
                    else
                    {
                        string precoProduto = $"- {opcao.Price.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs13 = $"{opcao.Quantity}X  {opcao.Name} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs13);
                    }

                    continue;
                }

                if (obs14 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.ExternalCode);

                    if (pesquisaProduto)
                    {
                        obs14 = $"{opcao.Quantity}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.ExternalCode)}";
                        ClasseDeSuporte.Observações.Add(obs14);
                    }
                    else
                    {
                        string precoProduto = $"- {opcao.Price.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs14 = $"{opcao.Quantity}X  {opcao.Name} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs14);
                    }

                    continue;
                }


            }


            ClasseDeSuporte.Tamanho = "U";

            ClasseDeSuporte.ExternalCode1 = externalCode1;
            ClasseDeSuporte.ExternalCode2 = externalCode2;
            ClasseDeSuporte.ExternalCode3 = externalCode3;

            ClasseDeSuporte.Obs1 = obs1;
            ClasseDeSuporte.Obs2 = obs2;
            ClasseDeSuporte.Obs3 = obs3;
            ClasseDeSuporte.Obs4 = obs4;
            ClasseDeSuporte.Obs5 = obs5;
            ClasseDeSuporte.Obs6 = obs6;
            ClasseDeSuporte.Obs7 = obs7;
            ClasseDeSuporte.Obs8 = obs8;
            ClasseDeSuporte.Obs9 = obs9;
            ClasseDeSuporte.Obs10 = obs10;
            ClasseDeSuporte.Obs11 = obs11;
            ClasseDeSuporte.Obs12 = obs12;
            ClasseDeSuporte.Obs13 = obs13;
            ClasseDeSuporte.Obs14 = obs14;

            return ClasseDeSuporte;

        }
    }


}

//Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\SAAB\BASE\CONTAS.mdb
