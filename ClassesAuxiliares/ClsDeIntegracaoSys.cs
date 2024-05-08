using SysIntegradorApp.data;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

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
     string? obsConta2 = null,
     string? endEntrega = "RETIRADA",
     string? bairEntrega = "RETIRADA",
     string? entregador = "RETIRADA"
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

                string sqlInsert = $"INSERT INTO Sequencia (MESA, STATUS,CORTESIA ,TAXAENTREGA,TAXAMOTOBOY, DTINICIO, HRINICIO, ENDENTREGA, BAIENTREGA, CONTATO, ENTREGADOR, USUARIO, DTSAIDA, HRSAIDA, OBSCONTA1, OBSCONTA2, iFoodPedidoID) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?,?)";

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
            MessageBox.Show(ex.ToString(), "ERRO AO INSERIR O PEDIDO NO BANCO DE DADOS DO ACCESS");
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
                        case "BANK_PAY ":
                            tipoPagamento = "PAGCRT";
                            break;
                        case "FOOD_VOUCHER ":
                            tipoPagamento = "PAGCRT";
                            break;
                        default:
                            tipoPagamento = "PAGCRT";
                            break;
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

    public static void IntegracaoContas(int conta, //numero
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
      string? usuario = "CAIXA"
      )
    { //aqui começa o código para inserção na tabela CONTAS
        try
        {
            string banco = CaminhoBaseSysMenu;//@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\gui-c\OneDrive\Área de Trabalho\SysIntegrador\CONTAS.mdb";

            using (OleDbConnection connection = new OleDbConnection(banco))
            {
                connection.Open();

                string sqlInsert = $"INSERT INTO Contas (CONTA,MESA,QTDADE,CODCARDA1,CODCARDA2,CODCARDA3,TAMANHO,DESCARDA,VALORUNIT,VALORTOTAL,DATAINICIO,HORAINICIO,OBS1,OBS2,OBS3,OBS4,OBS5,OBS6,OBS7,OBS8,OBS9,OBS10,OBS11,OBS12,OBS13,OBS14,OBS15,CLIENTE,STATUS,TELEFONE,IMPCOMANDA,IMPCOMANDA2,QTDCOMANDA,USUARIO ) VALUES (?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?)";

                using (OleDbCommand command = new OleDbCommand(sqlInsert, connection))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.ForegroundColor = ConsoleColor.White;
                    // Parâmetros para a consulta SQL
                    command.Parameters.AddWithValue("@CONTA", conta);
                    command.Parameters.AddWithValue("@MESA", mesa);
                    command.Parameters.AddWithValue("@QTDADE", qtdade);
                    command.Parameters.AddWithValue("@CODCARDA1", codCarda1);
                    command.Parameters.AddWithValue("@CODCARDA2", codCarda2);
                    command.Parameters.AddWithValue("@CODCARDA3", codCarda3);
                    command.Parameters.AddWithValue("@TAMANHO", tamanho);
                    command.Parameters.AddWithValue("@DESCARDA", descarda);
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
                    command.Parameters.AddWithValue("@CLIENTE", cliente);
                    command.Parameters.AddWithValue("@STATUS", "P");
                    command.Parameters.AddWithValue("@TELEFONE", telefone);
                    command.Parameters.AddWithValue("@IMPCOMANDA", impComanda);
                    command.Parameters.AddWithValue("@IMPCOMANDA2", ImpComanda2);
                    command.Parameters.AddWithValue("@QTDCOMANDA", qtdComanda);
                    command.Parameters.AddWithValue("@USUARIO", usuario);


                    // Executa o comando SQL
                    int rowsAffected = command.ExecuteNonQuery();

                }//fechamento chave segundo using 
            }//fechamento chave primeiro using
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString(), "Ops");
        }
    }


    public static void IntegracaoPagCartao(PedidoCompleto pedidoCompleto, int NumContas)
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

                string? tipoPagamento = DefinePagamento(pedidoCompleto.payments.methods[0].method);


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

            using (OleDbConnection connection = new OleDbConnection(banco))
            {
                connection.Open();

                float valorPagamento = pedidoCompleto.payments.methods[0].value;

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
        catch (Exception ex)
        {
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

    public static void CadastraCliente(Customer cliente, Delivery entrega)
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
            MessageBox.Show(ex.ToString(), "Problema em procurar cliente");
        }
    }

    public static string DefinePagamento(string? tipoPagamento) //define o nome do pagamento para inserir no pagCartão
    {
        string pagamento = "";

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
            case "BANK_PAY ":
                pagamento = "Débito";
                break;
            case "FOOD_VOUCHER ":
                pagamento = "Débito";
                break;
            case "OTHER":
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
                return locaisImp;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString(), "Erro ao definir local de impresão.");
        }
        return locaisImp;
    }

    public static bool VerificaCaixaAberto()
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
            MessageBox.Show(ex.Message, "Ops");
        }
        return CaixaAberto;
    }


    public static void ExcluiPedidoCasoCancelado(string orderId)
    {
        try
        {
            ApplicationDbContext dbPostgres = new ApplicationDbContext();
            var pedido = dbPostgres.parametrosdopedido.Where(x => x.Id == orderId).ToList().FirstOrDefault();
            int NumConta = pedido.Conta;
            string banco = CaminhoBaseSysMenu;

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


}

//Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\SAAB\BASE\CONTAS.mdb
