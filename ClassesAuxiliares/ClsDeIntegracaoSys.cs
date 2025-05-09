﻿using SysIntegradorApp.data;
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
using SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoCCM;
using SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoAnotaAi;
using SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoTaxyMachine;
using SysIntegradorApp.ClassesAuxiliares.ClassesGarcomSysMenu;
using System.Reflection.Metadata.Ecma335;
using SysIntegradorApp.Forms;
using SysIntegradorApp.ClassesAuxiliares.ClassesAiqfome;

namespace SysIntegradorApp.ClassesAuxiliares;

public class ClsDeIntegracaoSys
{
    public static string? CaminhoBaseSysMenu { get; set; } = ApplicationDbContext.RetornaCaminhoBaseSysMenu();

    public readonly IMeuContexto _Context;

    public ClsDeIntegracaoSys()
    {

    }

    public ClsDeIntegracaoSys(MeuContexto context)
    {
        _Context = context;
    }

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
     string? entregador = "0",
     bool eIfood = false,
     bool eDelMatch = false,
     bool eOnpedido = false,
     bool eAiQFome = false,
     bool eCCM = false,
     bool eAnotaAi = false,
     string? telefone = " ",
     string? status = "P",
     float Couvert = 0f
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
                if (telefone.Length > 14)
                {
                    telefone = telefone.Replace(" ", "");
                }

                if (telefone is null)
                    telefone = " ";

                connection.Open();

                string sqlInsert = $"INSERT INTO Sequencia (MESA, STATUS,CORTESIA, COUVERT,TAXAENTREGA,TAXAMOTOBOY, DTINICIO, HRINICIO,TELEFONE,FONEPEDIDO ,ENDENTREGA, BAIENTREGA, REFENTREGA ,CONTATO, CLIENTE ,ENTREGADOR, USUARIO, DTSAIDA, HRSAIDA, OBSCONTA1, OBSCONTA2 ,iFoodPedidoID) VALUES (?,?,?,?, ?, ?,?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?,?)";

                using (OleDbCommand command = new OleDbCommand(sqlInsert, connection))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.ForegroundColor = ConsoleColor.White;
                    // Parâmetros para a consulta SQL
                    command.Parameters.AddWithValue("@MESA", mesa);
                    command.Parameters.AddWithValue("@STATUS", status);

                    if (!eIfood)
                        command.Parameters.AddWithValue("@CORTESIA", cortesia);
                    else
                        command.Parameters.AddWithValue("@CORTESIA", 0.0f);


                    command.Parameters.AddWithValue("@COUVERT", Couvert);
                    command.Parameters.AddWithValue("@TAXAENTREGA", taxaEntrega);
                    command.Parameters.AddWithValue("@TAXAMOTOBOY", taxaMotoboy);
                    command.Parameters.AddWithValue("@DTINICIO", dtInicio.Replace("-", "/"));
                    command.Parameters.AddWithValue("@HRINICIO", hrInicio);
                    command.Parameters.AddWithValue("@TELEFONE", telefone.Length > 14 ? telefone.Substring(0, 14) : telefone);
                    command.Parameters.AddWithValue("@FONEPEDIDO", telefone.Length > 14 ? telefone.Substring(0, 14) : telefone);
                    command.Parameters.AddWithValue("@ENDENTREGA", endEntrega); //se vier WEBB aqui vai ser null
                    command.Parameters.AddWithValue("@BAIENTREGA", bairEntrega);//se vier WEBB aqui vai ser null
                    command.Parameters.AddWithValue("@REFENTREGA", referencia);//se vier WEBB aqui vai ser null
                    command.Parameters.AddWithValue("@CONTATO", contatoNome.Length > 50 ? contatoNome.Substring(0, 40) : contatoNome);
                    command.Parameters.AddWithValue("@CLIENTE", contatoNome.Length > 40 ? contatoNome.Substring(0, 40) : contatoNome);
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
                                    
                                    if (eAiQFome)
                                    {
                                        string query = "UPDATE Sequencia SET PEDWEB = ? WHERE CONTA = ?";
                                        using (OleDbCommand ComandoMudaIsIfood = new OleDbCommand(query, connection))
                                        {

                                            OleDbParameter paramConta = new OleDbParameter("@CONTA", OleDbType.VarChar);
                                            paramConta.Value = ultimoNumeroConta.ToString();

                                            OleDbParameter paramPedWeb = new OleDbParameter("@PEDWEB", "AIQFOME");

                                            ComandoMudaIsIfood.Parameters.Add(paramPedWeb);
                                            ComandoMudaIsIfood.Parameters.Add(paramConta);

                                            ComandoMudaIsIfood.ExecuteNonQuery();
                                        }
                                    }

                                    if (eAnotaAi)
                                    {
                                        string query = "UPDATE Sequencia SET PEDWEB = ? WHERE CONTA = ?";
                                        using (OleDbCommand ComandoMudaIsIfood = new OleDbCommand(query, connection))
                                        {

                                            OleDbParameter paramConta = new OleDbParameter("@CONTA", OleDbType.VarChar);
                                            paramConta.Value = ultimoNumeroConta.ToString();

                                            OleDbParameter paramPedWeb = new OleDbParameter("@PEDWEB", "AnotaAi");

                                            ComandoMudaIsIfood.Parameters.Add(paramPedWeb);
                                            ComandoMudaIsIfood.Parameters.Add(paramConta);

                                            ComandoMudaIsIfood.ExecuteNonQuery();
                                        }
                                    }


                                    if (eCCM)
                                    {
                                        string query = "UPDATE Sequencia SET PEDWEB = ? WHERE CONTA = ?";
                                        using (OleDbCommand ComandoMudaIsIfood = new OleDbCommand(query, connection))
                                        {

                                            OleDbParameter paramConta = new OleDbParameter("@CONTA", OleDbType.VarChar);
                                            paramConta.Value = ultimoNumeroConta.ToString();

                                            OleDbParameter paramPedWeb = new OleDbParameter("@PEDWEB", "CCM");

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
            await SysAlerta.Alerta("Erro ao processar pedido", ex.Message, SysAlertaTipo.Erro, SysAlertaButtons.Ok);
        }

        return ultimoNumeroConta;
    }

    public static void UpdateMeiosDePagamentosSequenciaParaOIfood(Payments pagamento, int numConta, float desconto = 0.0f, float acrecimo = 0.0f, List<Benefits>? benefits = null)
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
                            tipoPagamento = "PAGCRT";
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
                        case "money":
                            tipoPagamento = "PAGDNH";
                            break;
                        case "DIGITAL_WALLET":
                            tipoPagamento = "PAGCRT";
                            break;
                        default:
                            tipoPagamento = "PAGCRT";
                            break;
                    }

                    if (pagamentoAtual.type == "ONLINE")
                    {
                        string SelectQuery = "SELECT * FROM Sequencia WHERE CONTA = @CONDICAO"; //Vai fazer um select primeiro na coluna para ver se ela tem valor
                        float ValorDaColuna = 0.0f;

                        using (OleDbCommand selectCommand = new OleDbCommand(SelectQuery, connection))
                        {
                            selectCommand.Parameters.AddWithValue("@CONDICAO", numConta);

                            // Executar a consulta SELECT
                            using (OleDbDataReader reader = selectCommand.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    ValorDaColuna = Convert.ToSingle(reader["PAGONLINE"].ToString());
                                }
                            }
                        } // aqui faz o select e coloca o valor da coluna PAGONLINE

                        string updateQuery = $"UPDATE Sequencia SET PAGONLINE = @NovoValor WHERE CONTA = @CONDICAO;";
                        double valor = pagamentoAtual.value;

                        if (ValorDaColuna > 0)
                            valor += ValorDaColuna; // aqui verifica se o valor que ja estava na coluna é maior que zero, se for ele soma o valor do pagamento atual

                        using (OleDbCommand command = new OleDbCommand(updateQuery, connection))
                        {
                            // Definindo os parâmetros para a instrução SQL
                            command.Parameters.AddWithValue($"@NovoValor", valor);
                            command.Parameters.AddWithValue("@CONDICAO", numConta);

                            // Executando o comando UPDATE
                            command.ExecuteNonQuery();
                        }


                        if (acrecimo > 0)
                        {
                            string QueryDeAdicionarDesconto = $"UPDATE Sequencia SET ACRESCIMO = @NovoValor WHERE CONTA = @CONDICAO;";
                            using (OleDbCommand command = new OleDbCommand(updateQuery, connection))
                            {
                                // Definindo os parâmetros para a instrução SQL
                                command.Parameters.AddWithValue($"@NovoValor", acrecimo);
                                command.Parameters.AddWithValue("@CONDICAO", numConta);

                                // Executando o comando UPDATE
                                command.ExecuteNonQuery();
                            }
                        }

                        if (benefits is not null)
                        {
                            foreach (var benefitMajor in benefits)
                            {
                                if (benefitMajor.value > 0)
                                {
                                    if (benefitMajor.sponsorshipValues.Count > 0)
                                    {
                                        //se cair no if ele adiciona os valores do array em VOUCHER
                                        foreach (var benefit in benefitMajor.sponsorshipValues)
                                        {
                                            if (benefit.name == "IFOOD")
                                            {
                                                if (benefit.value > 0)
                                                {
                                                    string QueryDeAdicionarVoucher = $"UPDATE Sequencia SET VOUCHER = @NovoValor WHERE CONTA = @CONDICAO;";
                                                    using (OleDbCommand command = new OleDbCommand(QueryDeAdicionarVoucher, connection))
                                                    {

                                                        command.Parameters.AddWithValue($"@NovoValor", benefit.value);
                                                        command.Parameters.AddWithValue("@CONDICAO", numConta);

                                                        // Executando o comando UPDATE
                                                        command.ExecuteNonQuery();
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (benefit.value > 0)
                                                {
                                                    string QueryDeAdicionarCortesia = $"UPDATE Sequencia SET CORTESIA = @NovoValor WHERE CONTA = @CONDICAO;";
                                                    using (OleDbCommand command = new OleDbCommand(QueryDeAdicionarCortesia, connection))
                                                    {

                                                        command.Parameters.AddWithValue($"@NovoValor", benefit.value);
                                                        command.Parameters.AddWithValue("@CONDICAO", numConta);

                                                        // Executando o comando UPDATE
                                                        command.ExecuteNonQuery();
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        //se cair no else ele adiciona o valor total em CORTESIA
                                        string QueryDeAdicionarDescontoCortesia = $"UPDATE Sequencia SET CORTESIA = @NovoValor WHERE CONTA = @CONDICAO;";
                                        using (OleDbCommand command = new OleDbCommand(QueryDeAdicionarDescontoCortesia, connection))
                                        {

                                            command.Parameters.AddWithValue($"@NovoValor", benefitMajor.value);
                                            command.Parameters.AddWithValue("@CONDICAO", numConta);

                                            // Executando o comando UPDATE
                                            command.ExecuteNonQuery();
                                        }
                                    }
                                }
                            }
                        }

                        continue;
                    }


                    if (tipoPagamento == "PAGCRT")
                    {
                        string SelectQuery = "SELECT * FROM Sequencia WHERE CONTA = @CONDICAO"; //Vai fazer um select primeiro na coluna para ver se ela tem valor
                        float ValorDaColuna = 0.0f;

                        using (OleDbCommand selectCommand = new OleDbCommand(SelectQuery, connection))
                        {
                            selectCommand.Parameters.AddWithValue("@CONDICAO", numConta);

                            // Executar a consulta SELECT
                            using (OleDbDataReader reader = selectCommand.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    ValorDaColuna = Convert.ToSingle(reader["PAGCRT"].ToString());
                                }
                            }
                        } // aqui faz o select e coloca o valor da coluna PAGCARTAO

                        string updateQuery = $"UPDATE Sequencia SET PAGCRT = @NovoValor WHERE CONTA = @CONDICAO;";
                        double valor = pagamentoAtual.value;

                        if (ValorDaColuna > 0)
                            valor += ValorDaColuna; // aqui verifica se o valor que ja estava na coluna é maior que zero, se for ele soma o valor do pagamento atual


                        using (OleDbCommand command = new OleDbCommand(updateQuery, connection))
                        {

                            // Definindo os parâmetros para a instrução SQL
                            command.Parameters.AddWithValue($"@NovoValor", valor);
                            command.Parameters.AddWithValue("@CONDICAO", numConta);

                            // Executando o comando UPDATE
                            command.ExecuteNonQuery();
                        }


                        if (acrecimo > 0)
                        {
                            string QueryDeAdicionarAcrecimo = $"UPDATE Sequencia SET ACRESCIMO = @NovoValor WHERE CONTA = @CONDICAO;";
                            using (OleDbCommand command = new OleDbCommand(QueryDeAdicionarAcrecimo, connection))
                            {
                                // Definindo os parâmetros para a instrução SQL
                                command.Parameters.AddWithValue($"@NovoValor", acrecimo);
                                command.Parameters.AddWithValue("@CONDICAO", numConta);

                                // Executando o comando UPDATE
                                command.ExecuteNonQuery();
                            }
                        }

                        if (benefits is not null)
                        {
                            foreach (var benefitMajor in benefits)
                            {
                                if (benefitMajor.value > 0)
                                {
                                    if (benefitMajor.sponsorshipValues.Count > 0)
                                    {
                                        //se cair no if ele adiciona os valores do array em VOUCHER
                                        foreach (var benefit in benefitMajor.sponsorshipValues)
                                        {
                                            if (benefit.name == "IFOOD")
                                            {
                                                if (benefit.value > 0)
                                                {
                                                    string QueryDeAdicionarVoucher = $"UPDATE Sequencia SET VOUCHER = @NovoValor WHERE CONTA = @CONDICAO;";
                                                    using (OleDbCommand command = new OleDbCommand(QueryDeAdicionarVoucher, connection))
                                                    {

                                                        command.Parameters.AddWithValue($"@NovoValor", benefit.value);
                                                        command.Parameters.AddWithValue("@CONDICAO", numConta);

                                                        // Executando o comando UPDATE
                                                        command.ExecuteNonQuery();
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (benefit.value > 0)
                                                {
                                                    string QueryDeAdicionarCortesia = $"UPDATE Sequencia SET CORTESIA = @NovoValor WHERE CONTA = @CONDICAO;";
                                                    using (OleDbCommand command = new OleDbCommand(QueryDeAdicionarCortesia, connection))
                                                    {

                                                        command.Parameters.AddWithValue($"@NovoValor", benefit.value);
                                                        command.Parameters.AddWithValue("@CONDICAO", numConta);

                                                        // Executando o comando UPDATE
                                                        command.ExecuteNonQuery();
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        //se cair no else ele adiciona o valor total em CORTESIA
                                        string QueryDeAdicionarDescontoCortesia = $"UPDATE Sequencia SET CORTESIA = @NovoValor WHERE CONTA = @CONDICAO;";
                                        using (OleDbCommand command = new OleDbCommand(QueryDeAdicionarDescontoCortesia, connection))
                                        {

                                            command.Parameters.AddWithValue($"@NovoValor", benefitMajor.value);
                                            command.Parameters.AddWithValue("@CONDICAO", numConta);

                                            // Executando o comando UPDATE
                                            command.ExecuteNonQuery();
                                        }
                                    }
                                }
                            }
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


                        if (acrecimo > 0)
                        {
                            string QueryDeAdicionarAcrecimo = $"UPDATE Sequencia SET ACRESCIMO = @NovoValor WHERE CONTA = @CONDICAO;";
                            using (OleDbCommand command = new OleDbCommand(QueryDeAdicionarAcrecimo, connection))
                            {
                                // Definindo os parâmetros para a instrução SQL
                                command.Parameters.AddWithValue($"@NovoValor", acrecimo);
                                command.Parameters.AddWithValue("@CONDICAO", numConta);

                                // Executando o comando UPDATE
                                command.ExecuteNonQuery();
                            }
                        }

                        if (benefits is not null)
                        {
                            foreach (var benefitMajor in benefits)
                            {
                                if (benefitMajor.value > 0)
                                {
                                    if (benefitMajor.sponsorshipValues.Count > 0)
                                    {
                                        //se cair no if ele adiciona os valores do array em VOUCHER
                                        foreach (var benefit in benefitMajor.sponsorshipValues)
                                        {
                                            if (benefit.name == "IFOOD")
                                            {
                                                if (benefit.value > 0)
                                                {
                                                    string QueryDeAdicionarVoucher = $"UPDATE Sequencia SET VOUCHER = @NovoValor WHERE CONTA = @CONDICAO;";
                                                    using (OleDbCommand command = new OleDbCommand(QueryDeAdicionarVoucher, connection))
                                                    {

                                                        command.Parameters.AddWithValue($"@NovoValor", benefit.value);
                                                        command.Parameters.AddWithValue("@CONDICAO", numConta);

                                                        // Executando o comando UPDATE
                                                        command.ExecuteNonQuery();
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (benefit.value > 0)
                                                {
                                                    string QueryDeAdicionarCortesia = $"UPDATE Sequencia SET CORTESIA = @NovoValor WHERE CONTA = @CONDICAO;";
                                                    using (OleDbCommand command = new OleDbCommand(QueryDeAdicionarCortesia, connection))
                                                    {

                                                        command.Parameters.AddWithValue($"@NovoValor", benefit.value);
                                                        command.Parameters.AddWithValue("@CONDICAO", numConta);

                                                        // Executando o comando UPDATE
                                                        command.ExecuteNonQuery();
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        //se cair no else ele adiciona o valor total em CORTESIA
                                        string QueryDeAdicionarDescontoCortesia = $"UPDATE Sequencia SET CORTESIA = @NovoValor WHERE CONTA = @CONDICAO;";
                                        using (OleDbCommand command = new OleDbCommand(QueryDeAdicionarDescontoCortesia, connection))
                                        {

                                            command.Parameters.AddWithValue($"@NovoValor", benefitMajor.value);
                                            command.Parameters.AddWithValue("@CONDICAO", numConta);

                                            // Executando o comando UPDATE
                                            command.ExecuteNonQuery();
                                        }
                                    }
                                }
                            }
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

                        if (acrecimo > 0)
                        {
                            string QueryDeAdicionarAcrecimo = $"UPDATE Sequencia SET ACRESCIMO = @NovoValor WHERE CONTA = @CONDICAO;";
                            using (OleDbCommand command = new OleDbCommand(QueryDeAdicionarAcrecimo, connection))
                            {
                                // Definindo os parâmetros para a instrução SQL
                                command.Parameters.AddWithValue($"@NovoValor", acrecimo);
                                command.Parameters.AddWithValue("@CONDICAO", numConta);

                                // Executando o comando UPDATE
                                command.ExecuteNonQuery();
                            }
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

                        if (acrecimo > 0)
                        {
                            string QueryDeAdicionarAcrecimo = $"UPDATE Sequencia SET ACRESCIMO = @NovoValor WHERE CONTA = @CONDICAO;";
                            using (OleDbCommand command = new OleDbCommand(QueryDeAdicionarAcrecimo, connection))
                            {
                                // Definindo os parâmetros para a instrução SQL
                                command.Parameters.AddWithValue($"@NovoValor", acrecimo);
                                command.Parameters.AddWithValue("@CONDICAO", numConta);

                                // Executando o comando UPDATE
                                command.ExecuteNonQuery();
                            }
                        }

                        if (benefits is not null)
                        {
                            foreach (var benefitMajor in benefits)
                            {
                                if (benefitMajor.value > 0)
                                {
                                    if (benefitMajor.sponsorshipValues.Count > 0)
                                    {
                                        //se cair no if ele adiciona os valores do array em VOUCHER
                                        foreach (var benefit in benefitMajor.sponsorshipValues)
                                        {
                                            if (benefit.name == "IFOOD")
                                            {
                                                if (benefit.value > 0)
                                                {
                                                    string QueryDeAdicionarVoucher = $"UPDATE Sequencia SET VOUCHER = @NovoValor WHERE CONTA = @CONDICAO;";
                                                    using (OleDbCommand command = new OleDbCommand(QueryDeAdicionarVoucher, connection))
                                                    {

                                                        command.Parameters.AddWithValue($"@NovoValor", benefit.value);
                                                        command.Parameters.AddWithValue("@CONDICAO", numConta);

                                                        // Executando o comando UPDATE
                                                        command.ExecuteNonQuery();
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (benefit.value > 0)
                                                {
                                                    string QueryDeAdicionarCortesia = $"UPDATE Sequencia SET CORTESIA = @NovoValor WHERE CONTA = @CONDICAO;";
                                                    using (OleDbCommand command = new OleDbCommand(QueryDeAdicionarCortesia, connection))
                                                    {

                                                        command.Parameters.AddWithValue($"@NovoValor", benefit.value);
                                                        command.Parameters.AddWithValue("@CONDICAO", numConta);

                                                        // Executando o comando UPDATE
                                                        command.ExecuteNonQuery();
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        //se cair no else ele adiciona o valor total em CORTESIA
                                        string QueryDeAdicionarDescontoCortesia = $"UPDATE Sequencia SET CORTESIA = @NovoValor WHERE CONTA = @CONDICAO;";
                                        using (OleDbCommand command = new OleDbCommand(QueryDeAdicionarDescontoCortesia, connection))
                                        {

                                            command.Parameters.AddWithValue($"@NovoValor", benefitMajor.value);
                                            command.Parameters.AddWithValue("@CONDICAO", numConta);

                                            // Executando o comando UPDATE
                                            command.ExecuteNonQuery();
                                        }
                                    }
                                }
                            }
                        }

                        continue;
                    }

                    if (tipoPagamento == "Dinheiro")
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


                        if (acrecimo > 0)
                        {
                            string QueryDeAdicionarAcrecimo = $"UPDATE Sequencia SET ACRESCIMO = @NovoValor WHERE CONTA = @CONDICAO;";
                            using (OleDbCommand command = new OleDbCommand(QueryDeAdicionarAcrecimo, connection))
                            {
                                // Definindo os parâmetros para a instrução SQL
                                command.Parameters.AddWithValue($"@NovoValor", acrecimo);
                                command.Parameters.AddWithValue("@CONDICAO", numConta);

                                // Executando o comando UPDATE
                                command.ExecuteNonQuery();
                            }
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

    public static void UpdateMeiosDePagamentosSequencia(Payments pagamento, int numConta, float desconto = 0.0f, float acrecimo = 0.0f, List<Benefits>? benefits = null)
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
                            tipoPagamento = "PAGCRT";
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
                        case "Outros":
                            tipoPagamento = "PAGDNH";
                            break;
                        case "money":
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


                        if (acrecimo > 0)
                        {
                            string QueryDeAdicionarDesconto = $"UPDATE Sequencia SET ACRESCIMO = @NovoValor WHERE CONTA = @CONDICAO;"; //ACRESCIMO
                            using (OleDbCommand command = new OleDbCommand(QueryDeAdicionarDesconto, connection))
                            {
                                // Definindo os parâmetros para a instrução SQL
                                command.Parameters.AddWithValue($"@NovoValor", acrecimo);
                                command.Parameters.AddWithValue("@CONDICAO", numConta);

                                // Executando o comando UPDATE
                                command.ExecuteNonQuery();
                            }
                        }

                        if (benefits is not null)
                        {
                            foreach (var benefitMajor in benefits)
                            {
                                if (benefitMajor.value > 0)
                                {
                                    if (benefitMajor.sponsorshipValues.Count > 0)
                                    {
                                        //se cair no if ele adiciona os valores do array em VOUCHER
                                        foreach (var benefit in benefitMajor.sponsorshipValues)
                                        {
                                            if (benefit.name == "IFOOD")
                                            {
                                                if (benefit.value > 0)
                                                {
                                                    string QueryDeAdicionarVoucher = $"UPDATE Sequencia SET VOUCHER = @NovoValor WHERE CONTA = @CONDICAO;";
                                                    using (OleDbCommand command = new OleDbCommand(QueryDeAdicionarVoucher, connection))
                                                    {

                                                        command.Parameters.AddWithValue($"@NovoValor", benefit.value);
                                                        command.Parameters.AddWithValue("@CONDICAO", numConta);

                                                        // Executando o comando UPDATE
                                                        command.ExecuteNonQuery();
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (benefit.value > 0)
                                                {
                                                    string QueryDeAdicionarCortesia = $"UPDATE Sequencia SET CORTESIA = @NovoValor WHERE CONTA = @CONDICAO;";
                                                    using (OleDbCommand command = new OleDbCommand(QueryDeAdicionarCortesia, connection))
                                                    {

                                                        command.Parameters.AddWithValue($"@NovoValor", benefit.value);
                                                        command.Parameters.AddWithValue("@CONDICAO", numConta);

                                                        // Executando o comando UPDATE
                                                        command.ExecuteNonQuery();
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        //se cair no else ele adiciona o valor total em CORTESIA
                                        string QueryDeAdicionarDescontoCortesia = $"UPDATE Sequencia SET CORTESIA = @NovoValor WHERE CONTA = @CONDICAO;";
                                        using (OleDbCommand command = new OleDbCommand(QueryDeAdicionarDescontoCortesia, connection))
                                        {

                                            command.Parameters.AddWithValue($"@NovoValor", benefitMajor.value);
                                            command.Parameters.AddWithValue("@CONDICAO", numConta);

                                            // Executando o comando UPDATE
                                            command.ExecuteNonQuery();
                                        }
                                    }
                                }
                            }
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


                        if (acrecimo > 0)
                        {
                            string QueryDeAdicionarAcrecimo = $"UPDATE Sequencia SET ACRESCIMO = @NovoValor WHERE CONTA = @CONDICAO;";
                            using (OleDbCommand command = new OleDbCommand(QueryDeAdicionarAcrecimo, connection))
                            {
                                // Definindo os parâmetros para a instrução SQL
                                command.Parameters.AddWithValue($"@NovoValor", acrecimo);
                                command.Parameters.AddWithValue("@CONDICAO", numConta);

                                // Executando o comando UPDATE
                                command.ExecuteNonQuery();
                            }
                        }

                        if (benefits is not null)
                        {
                            foreach (var benefitMajor in benefits)
                            {
                                if (benefitMajor.value > 0)
                                {
                                    if (benefitMajor.sponsorshipValues.Count > 0)
                                    {
                                        //se cair no if ele adiciona os valores do array em VOUCHER
                                        foreach (var benefit in benefitMajor.sponsorshipValues)
                                        {
                                            if (benefit.name == "IFOOD")
                                            {
                                                if (benefit.value > 0)
                                                {
                                                    string QueryDeAdicionarVoucher = $"UPDATE Sequencia SET VOUCHER = @NovoValor WHERE CONTA = @CONDICAO;";
                                                    using (OleDbCommand command = new OleDbCommand(QueryDeAdicionarVoucher, connection))
                                                    {

                                                        command.Parameters.AddWithValue($"@NovoValor", benefit.value);
                                                        command.Parameters.AddWithValue("@CONDICAO", numConta);

                                                        // Executando o comando UPDATE
                                                        command.ExecuteNonQuery();
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (benefit.value > 0)
                                                {
                                                    string QueryDeAdicionarCortesia = $"UPDATE Sequencia SET CORTESIA = @NovoValor WHERE CONTA = @CONDICAO;";
                                                    using (OleDbCommand command = new OleDbCommand(QueryDeAdicionarCortesia, connection))
                                                    {

                                                        command.Parameters.AddWithValue($"@NovoValor", benefit.value);
                                                        command.Parameters.AddWithValue("@CONDICAO", numConta);

                                                        // Executando o comando UPDATE
                                                        command.ExecuteNonQuery();
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        //se cair no else ele adiciona o valor total em CORTESIA
                                        string QueryDeAdicionarDescontoCortesia = $"UPDATE Sequencia SET CORTESIA = @NovoValor WHERE CONTA = @CONDICAO;";
                                        using (OleDbCommand command = new OleDbCommand(QueryDeAdicionarDescontoCortesia, connection))
                                        {

                                            command.Parameters.AddWithValue($"@NovoValor", benefitMajor.value);
                                            command.Parameters.AddWithValue("@CONDICAO", numConta);

                                            // Executando o comando UPDATE
                                            command.ExecuteNonQuery();
                                        }
                                    }
                                }
                            }
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


                        if (acrecimo > 0)
                        {
                            string QueryDeAdicionarAcrecimo = $"UPDATE Sequencia SET ACRESCIMO = @NovoValor WHERE CONTA = @CONDICAO;";
                            using (OleDbCommand command = new OleDbCommand(QueryDeAdicionarAcrecimo, connection))
                            {
                                // Definindo os parâmetros para a instrução SQL
                                command.Parameters.AddWithValue($"@NovoValor", acrecimo);
                                command.Parameters.AddWithValue("@CONDICAO", numConta);

                                // Executando o comando UPDATE
                                command.ExecuteNonQuery();
                            }
                        }

                        if (benefits is not null)
                        {
                            foreach (var benefitMajor in benefits)
                            {
                                if (benefitMajor.value > 0)
                                {
                                    if (benefitMajor.sponsorshipValues.Count > 0)
                                    {
                                        //se cair no if ele adiciona os valores do array em VOUCHER
                                        foreach (var benefit in benefitMajor.sponsorshipValues)
                                        {
                                            if (benefit.name == "IFOOD")
                                            {
                                                if (benefit.value > 0)
                                                {
                                                    string QueryDeAdicionarVoucher = $"UPDATE Sequencia SET VOUCHER = @NovoValor WHERE CONTA = @CONDICAO;";
                                                    using (OleDbCommand command = new OleDbCommand(QueryDeAdicionarVoucher, connection))
                                                    {

                                                        command.Parameters.AddWithValue($"@NovoValor", benefit.value);
                                                        command.Parameters.AddWithValue("@CONDICAO", numConta);

                                                        // Executando o comando UPDATE
                                                        command.ExecuteNonQuery();
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (benefit.value > 0)
                                                {
                                                    string QueryDeAdicionarCortesia = $"UPDATE Sequencia SET CORTESIA = @NovoValor WHERE CONTA = @CONDICAO;";
                                                    using (OleDbCommand command = new OleDbCommand(QueryDeAdicionarCortesia, connection))
                                                    {

                                                        command.Parameters.AddWithValue($"@NovoValor", benefit.value);
                                                        command.Parameters.AddWithValue("@CONDICAO", numConta);

                                                        // Executando o comando UPDATE
                                                        command.ExecuteNonQuery();
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        //se cair no else ele adiciona o valor total em CORTESIA
                                        string QueryDeAdicionarDescontoCortesia = $"UPDATE Sequencia SET CORTESIA = @NovoValor WHERE CONTA = @CONDICAO;";
                                        using (OleDbCommand command = new OleDbCommand(QueryDeAdicionarDescontoCortesia, connection))
                                        {

                                            command.Parameters.AddWithValue($"@NovoValor", benefitMajor.value);
                                            command.Parameters.AddWithValue("@CONDICAO", numConta);

                                            // Executando o comando UPDATE
                                            command.ExecuteNonQuery();
                                        }
                                    }
                                }
                            }
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

                        if (acrecimo > 0)
                        {
                            string QueryDeAdicionarAcrecimo = $"UPDATE Sequencia SET ACRESCIMO = @NovoValor WHERE CONTA = @CONDICAO;";
                            using (OleDbCommand command = new OleDbCommand(QueryDeAdicionarAcrecimo, connection))
                            {
                                // Definindo os parâmetros para a instrução SQL
                                command.Parameters.AddWithValue($"@NovoValor", acrecimo);
                                command.Parameters.AddWithValue("@CONDICAO", numConta);

                                // Executando o comando UPDATE
                                command.ExecuteNonQuery();
                            }
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

                        if (acrecimo > 0)
                        {
                            string QueryDeAdicionarAcrecimo = $"UPDATE Sequencia SET ACRESCIMO = @NovoValor WHERE CONTA = @CONDICAO;";
                            using (OleDbCommand command = new OleDbCommand(QueryDeAdicionarAcrecimo, connection))
                            {
                                // Definindo os parâmetros para a instrução SQL
                                command.Parameters.AddWithValue($"@NovoValor", acrecimo);
                                command.Parameters.AddWithValue("@CONDICAO", numConta);

                                // Executando o comando UPDATE
                                command.ExecuteNonQuery();
                            }
                        }

                        if (benefits is not null)
                        {
                            foreach (var benefitMajor in benefits)
                            {
                                if (benefitMajor.value > 0)
                                {
                                    if (benefitMajor.sponsorshipValues.Count > 0)
                                    {
                                        //se cair no if ele adiciona os valores do array em VOUCHER
                                        foreach (var benefit in benefitMajor.sponsorshipValues)
                                        {
                                            if (benefit.name == "IFOOD")
                                            {
                                                if (benefit.value > 0)
                                                {
                                                    string QueryDeAdicionarVoucher = $"UPDATE Sequencia SET VOUCHER = @NovoValor WHERE CONTA = @CONDICAO;";
                                                    using (OleDbCommand command = new OleDbCommand(QueryDeAdicionarVoucher, connection))
                                                    {

                                                        command.Parameters.AddWithValue($"@NovoValor", benefit.value);
                                                        command.Parameters.AddWithValue("@CONDICAO", numConta);

                                                        // Executando o comando UPDATE
                                                        command.ExecuteNonQuery();
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (benefit.value > 0)
                                                {
                                                    string QueryDeAdicionarCortesia = $"UPDATE Sequencia SET CORTESIA = @NovoValor WHERE CONTA = @CONDICAO;";
                                                    using (OleDbCommand command = new OleDbCommand(QueryDeAdicionarCortesia, connection))
                                                    {

                                                        command.Parameters.AddWithValue($"@NovoValor", benefit.value);
                                                        command.Parameters.AddWithValue("@CONDICAO", numConta);

                                                        // Executando o comando UPDATE
                                                        command.ExecuteNonQuery();
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        //se cair no else ele adiciona o valor total em CORTESIA
                                        string QueryDeAdicionarDescontoCortesia = $"UPDATE Sequencia SET CORTESIA = @NovoValor WHERE CONTA = @CONDICAO;";
                                        using (OleDbCommand command = new OleDbCommand(QueryDeAdicionarDescontoCortesia, connection))
                                        {

                                            command.Parameters.AddWithValue($"@NovoValor", benefitMajor.value);
                                            command.Parameters.AddWithValue("@CONDICAO", numConta);

                                            // Executando o comando UPDATE
                                            command.ExecuteNonQuery();
                                        }
                                    }
                                }
                            }
                        }

                        continue;
                    }

                    if (tipoPagamento == "Dinheiro")
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


                        if (acrecimo > 0)
                        {
                            string QueryDeAdicionarAcrecimo = $"UPDATE Sequencia SET ACRESCIMO = @NovoValor WHERE CONTA = @CONDICAO;";
                            using (OleDbCommand command = new OleDbCommand(QueryDeAdicionarAcrecimo, connection))
                            {
                                // Definindo os parâmetros para a instrução SQL
                                command.Parameters.AddWithValue($"@NovoValor", acrecimo);
                                command.Parameters.AddWithValue("@CONDICAO", numConta);

                                // Executando o comando UPDATE
                                command.ExecuteNonQuery();
                            }
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
      string? idPedido = " ",
      string? Requisicao = " ",
      string? HoraDeLancamentoDoItem = " ",
      string? garcom = " "
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

                string sqlInsert = $"INSERT INTO Contas (CONTA,MESA,QTDADE,CODCARDA1,CODCARDA2,CODCARDA3,TAMANHO,REQUISICAO,GARCON,DESCARDA,VALORUNIT,VALORTOTAL,DATAINICIO,HORAINICIO,OBS1,OBS2,OBS3,OBS4,OBS5,OBS6,OBS7,OBS8,OBS9,OBS10,OBS11,OBS12,OBS13,OBS14,OBS15,CLIENTE,STATUS,TELEFONE,IMPCOMANDA,IMPCOMANDA2,QTDCOMANDA,USUARIO,HORALANCAITEM ) VALUES (?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?)";

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
                    command.Parameters.AddWithValue("@REQUISICAO", Requisicao);
                    command.Parameters.AddWithValue("@GARCON", garcom);
                    command.Parameters.AddWithValue("@DESCARDA", descarda.Length > 31 ? descarda.Substring(0, 31) : descarda);
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
                    command.Parameters.AddWithValue("@CLIENTE", cliente.Length > 40 ? cliente.Substring(0, 40) : cliente);
                    command.Parameters.AddWithValue("@STATUS", status);
                    command.Parameters.AddWithValue("@TELEFONE", telefone.Length > 14 ? telefone.Substring(0, 14) : telefone);
                    command.Parameters.AddWithValue("@IMPCOMANDA", impComanda);
                    command.Parameters.AddWithValue("@IMPCOMANDA2", ImpComanda2);
                    command.Parameters.AddWithValue("@QTDCOMANDA", qtdComanda);
                    command.Parameters.AddWithValue("@USUARIO", usuario);
                    command.Parameters.AddWithValue("@HORALANCAITEM", HoraDeLancamentoDoItem);

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
            await SysAlerta.Alerta("Ops", $"{ex.Message}", SysAlertaTipo.Erro, SysAlertaButtons.Ok);
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

                string SqlSelectIntoCadastros = $"SELECT * FROM CARTAO WHERE NOME like '%{tipoPagamento}%'";

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

            if (type != "ONLINE" && type != "online" && metodo != "CASH" && metodo != "DIN" && metodo != "Dinheiro" && metodo != "money")
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

    public static async void CadastraClienteAnotaAi(SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoAnotaAi.CustomerAnotaAi cliente, SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoAnotaAi.EnderecoDeDelivery entrega, string? TelefoneCliente)
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

                string referenciaDoEndereco = entrega.Complement == null || entrega.Complement == "" ? " " : entrega.Complement;


                using (OleDbCommand command = new OleDbCommand(SqlSelectIntoCadastros, connection))
                {
                    command.Parameters.AddWithValue("@TELEFONE", TelefoneCliente);
                    command.Parameters.AddWithValue("@NOME", cliente.Nome);
                    command.Parameters.AddWithValue("@ENDERECO", entrega.FormattedAddress);
                    command.Parameters.AddWithValue("@BAIRRO", entrega.Neighborhood);
                    command.Parameters.AddWithValue("@CIDADE", entrega.City);
                    command.Parameters.AddWithValue("@ESTADO", "SP");
                    command.Parameters.AddWithValue("@CEP", entrega.PostalCode != null && entrega.PostalCode != "" ? entrega.PostalCode : " ");
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


    public static async void CadastraClienteCCM(SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoCCM.Cliente cliente, SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoCCM.Endereco entrega)
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

                string referenciaDoEndereco = entrega.Referencia == null || entrega.Referencia == "" ? " " : entrega.Referencia;

                string TelefoneCliente = cliente.Telefone;

                using (OleDbCommand command = new OleDbCommand(SqlSelectIntoCadastros, connection))
                {
                    command.Parameters.AddWithValue("@TELEFONE", TelefoneCliente);
                    command.Parameters.AddWithValue("@NOME", cliente.Nome);
                    command.Parameters.AddWithValue("@ENDERECO", $"{entrega.Rua}, {entrega.Numero} - {entrega.Bairro}");
                    command.Parameters.AddWithValue("@BAIRRO", entrega.Bairro);
                    command.Parameters.AddWithValue("@CIDADE", entrega.Cidade);
                    command.Parameters.AddWithValue("@ESTADO", "SP");
                    command.Parameters.AddWithValue("@CEP", entrega.Cep != null && entrega.Cep != "" ? entrega.Cep : " ");
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

        if (app == "ANOTAAI")
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

        if (app == "CCM")
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
                            ClassesAuxiliares.ClassesDeserializacaoDelmatch.Mesa mesa = new();

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


    public static ClsDeSuporteParaImpressaoDosItens DefineCaracteristicasDoItemOnPedido(itemsOn item, bool comanda = false, bool eIntegracao = false)
    {
        string? NomeProduto = "";
        ClsDeSuporteParaImpressaoDosItens ClasseDeSuporte = new ClsDeSuporteParaImpressaoDosItens();

        //Função que entra caso sejá pizza ou lanche ou porção, não mudei o nome do booleano pq já estava estruturado o cod 

        bool ePizza = item.externalCode == "G" || item.externalCode == "M" || item.externalCode == "P" || item.externalCode == "B" || item.externalCode == "LAN" || item.externalCode == "PRC" ? true : false;

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
                        if (!eIntegracao)
                        {
                            NomeProduto = $"{item.quantity}X " + ClsDeIntegracaoSys.NomeProdutoCardapio(option.externalCode);
                            externalCode1 = option.externalCode;
                        }
                        else
                        {
                            NomeProduto = ClsDeIntegracaoSys.NomeProdutoCardapio(option.externalCode);
                            externalCode1 = option.externalCode;
                        }
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

                        obs1 = $"{opcao.quantity}X {opcao.name} {precoProduto}";
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

                        obs2 = $"{opcao.quantity}X {opcao.name} {precoProduto}";
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

                        obs3 = $"{opcao.quantity}X {opcao.name} {precoProduto}";
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

                        obs4 = $"{opcao.quantity}X {opcao.name} {precoProduto}";
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

                        obs5 = $"{opcao.quantity}X {opcao.name} {precoProduto}";
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

                        obs6 = $"{opcao.quantity}X {opcao.name} {precoProduto}";
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

                        obs7 = $"{opcao.quantity}X {opcao.name} {precoProduto}";
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

                        obs8 = $"{opcao.quantity}X {opcao.name} {precoProduto}";
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

                        obs9 = $"{opcao.quantity}X {opcao.name} {precoProduto}";
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

                        obs10 = $"{opcao.quantity}X {opcao.name} {precoProduto}";
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

                        obs11 = $"{opcao.quantity}X {opcao.name} {precoProduto}";
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

                        obs12 = $"{opcao.quantity}X {opcao.name} {precoProduto}";
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

                        obs13 = $"{opcao.quantity}X {opcao.name} {precoProduto}";
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

                        obs14 = $"{opcao.quantity}X {opcao.name} {precoProduto}";
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
                if (!eIntegracao)
                {
                    NomeProduto = $"{item.quantity}X " + ClsDeIntegracaoSys.NomeProdutoCardapio(externalCode);
                    externalCode1 = externalCode;
                }
                else
                {
                    NomeProduto = ClsDeIntegracaoSys.NomeProdutoCardapio(externalCode);
                    externalCode1 = externalCode;
                }

            }
            else
            {
                NomeProduto = $"{item.quantity}X " + item.Name;
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

                        obs1 = $"{opcao.quantity}X {opcao.name} {precoProduto}";
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

                        obs2 = $"{opcao.quantity}X {opcao.name} {precoProduto}";
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

                        obs3 = $"{opcao.quantity}X {opcao.name} {precoProduto}";
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

                        obs4 = $"{opcao.quantity}X {opcao.name} {precoProduto}";
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

                        obs5 = $"{opcao.quantity}X {opcao.name} {precoProduto}";
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

                        obs6 = $"{opcao.quantity}X {opcao.name} {precoProduto}";
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

                        obs7 = $"{opcao.quantity}X {opcao.name} {precoProduto}";
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

                        obs8 = $"{opcao.quantity}X {opcao.name} {precoProduto}";
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

                        obs9 = $"{opcao.quantity}X {opcao.name} {precoProduto}";
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

                        obs10 = $"{opcao.quantity}X {opcao.name} {precoProduto}";
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

                        obs11 = $"{opcao.quantity}X {opcao.name} {precoProduto}";
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

                        obs12 = $"{opcao.quantity}X {opcao.name} {precoProduto}";
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

                        obs13 = $"{opcao.quantity}X {opcao.name} {precoProduto}";
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

                        obs14 = $"{opcao.quantity}X {opcao.name} {precoProduto}";
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

    public static ClsDeSuporteParaImpressaoDosItens DefineCaracteristicasDoItemAiQFome(ItemAiQFome item, bool comanda = false)
    {
        string? NomeProduto = "";
        ClsDeSuporteParaImpressaoDosItens ClasseDeSuporte = new ClsDeSuporteParaImpressaoDosItens();

        //Função que entra caso sejá pizza ou lanche ou porção, não mudei o nome do booleano pq já estava estruturado o cod 

        bool ePizza = item.Sku == "G" || item.Sku == "M" || item.Sku == "P" || item.Sku == "B" || item.Sku == "LAN" || item.Sku == "PRC" ? true : false;

        string? ObsDoItem = " ";

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

        string externalCode1 = " ";
        string externalCode2 = " ";
        string externalCode3 = " ";


        if (ePizza)
        {
            string? ePizza1 = null;
            string? ePizza2 = null;
            string? ePizza3 = null;


            if (item.Mandatory.Count > 0)
            {
                foreach (var option in item.Mandatory)
                {
                    bool eIncremento = option.Sku.Length == 3 ? true : false;

                    if (!option.Sku.Contains("m", StringComparison.OrdinalIgnoreCase) && !option.Sku.Contains("bd", StringComparison.OrdinalIgnoreCase) && !eIncremento && ePizza1 == null)
                    {
                        ePizza1 = option.Sku == "" ? " " : option.Sku;
                        bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(option.Sku);

                        if (pesquisaProduto)
                        {
                            NomeProduto = ClsDeIntegracaoSys.NomeProdutoCardapio(option.Sku);
                            externalCode1 = option.Sku;
                        }
                        else
                        {
                            NomeProduto += option.Name;
                        }

                        item.Mandatory.Remove(option);

                        continue;
                    }

                    if (!option.Sku.Contains("m", StringComparison.OrdinalIgnoreCase) && !option.Sku.Contains("bd", StringComparison.OrdinalIgnoreCase) && !eIncremento && ePizza2 == null)
                    {
                        ePizza2 = option.Sku == "" ? " " : option.Sku;
                        bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(option.Sku);

                        if (pesquisaProduto)
                        {
                            NomeProduto += " / " + ClsDeIntegracaoSys.NomeProdutoCardapio(option.Sku);
                            externalCode2 = option.Sku;
                        }
                        else
                        {
                            NomeProduto += " / " + option.Name;
                        }

                        item.Mandatory.Remove(option);

                        continue;
                    }

                    if (!option.Sku.Contains("m", StringComparison.OrdinalIgnoreCase) && !option.Sku.Contains("bd", StringComparison.OrdinalIgnoreCase) && !eIncremento && ePizza3 == null)
                    {
                        ePizza3 = option.Sku == "" ? " " : option.Sku;
                        bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(option.Sku);

                        if (pesquisaProduto)
                        {
                            NomeProduto += " / " + ClsDeIntegracaoSys.NomeProdutoCardapio(option.Sku);
                            externalCode3 = option.Sku;
                        }
                        else
                        {
                            NomeProduto += " / " + option.Name;
                        }

                        item.Mandatory.Remove(option);

                        continue;
                    }

                }

                if (NomeProduto == "")
                {
                    NomeProduto = item.Name;
                }
            }
            else
            {
                bool existeProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(item.Sku);

                if (existeProduto)
                {
                    NomeProduto = ClsDeIntegracaoSys.NomeProdutoCardapio(item.Sku);
                    externalCode1 = item.Sku;
                }
                else
                {
                    NomeProduto = item.Name;
                }

                if (item.Observations is not null && item.Observations != "")
                {
                    ObsDoItem = item.Observations;
                }

            }

            if (ObsDoItem.Length > 80)
                ObsDoItem = ObsDoItem.Substring(0, 80);

            ClasseDeSuporte.ObsDoItem = ObsDoItem;

            ClasseDeSuporte.Tamanho = item.Sku == "G" || item.Sku == "M" || item.Sku == "P" || item.Sku == "B" ? item.Sku : "U";
            ClasseDeSuporte.ExternalCode1 = externalCode1;
            ClasseDeSuporte.ExternalCode2 = externalCode2;
            ClasseDeSuporte.ExternalCode3 = externalCode3;


            List<string> ListaDeObservacoes = new List<string>() { obs1, obs2, obs3, obs4, obs5, obs6, obs7, obs8, obs9, obs10, obs11, obs12, obs13, obs14 };
            foreach (var opcao in item.Additional)
            {
                var ConversorfromAdicionalToMandatory = new Mandatory()
                {
                    Sku = opcao.Sku,
                    Name = opcao.Name,
                    Group = "",
                    Value = opcao.Value,
                };

                item.Mandatory.Add(ConversorfromAdicionalToMandatory);
            }

            foreach (Mandatory opcao in item.Mandatory)
            {
                for (int i = 0; i <= ListaDeObservacoes.Count; i++)
                {
                    if (ListaDeObservacoes[i] == " ")
                    {
                        bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.Sku);

                        if (pesquisaProduto)
                        {
                            ListaDeObservacoes[i] = ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.Sku);
                            ClasseDeSuporte.Observações!.Add(ListaDeObservacoes[i]);
                        }
                        else
                        {
                            ListaDeObservacoes[i] = opcao.Name;
                            ClasseDeSuporte.Observações!.Add(ListaDeObservacoes[i]);
                        }

                        break;
                    }

                }
            }

            ClasseDeSuporte.NomeProduto = NomeProduto;
            ClasseDeSuporte.Obs1 = ListaDeObservacoes[0];
            ClasseDeSuporte.Obs2 = ListaDeObservacoes[1];
            ClasseDeSuporte.Obs3 = ListaDeObservacoes[2];
            ClasseDeSuporte.Obs4 = ListaDeObservacoes[3];
            ClasseDeSuporte.Obs5 = ListaDeObservacoes[4];
            ClasseDeSuporte.Obs6 = ListaDeObservacoes[5];
            ClasseDeSuporte.Obs7 = ListaDeObservacoes[6];
            ClasseDeSuporte.Obs8 = ListaDeObservacoes[7];
            ClasseDeSuporte.Obs9 = ListaDeObservacoes[8];
            ClasseDeSuporte.Obs10 = ListaDeObservacoes[9];
            ClasseDeSuporte.Obs11 = ListaDeObservacoes[10];
            ClasseDeSuporte.Obs12 = ListaDeObservacoes[11];
            ClasseDeSuporte.Obs13 = ListaDeObservacoes[12];
            ClasseDeSuporte.Obs14 = ListaDeObservacoes[13];

            return ClasseDeSuporte;

        }
        else
        {
            string? externalCode = item.Sku == null || item.Sku == "" ? " " : item.Sku;
            bool existeProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(externalCode);

            if (existeProduto)
            {
                NomeProduto = $"{item.Quantity}X " + ClsDeIntegracaoSys.NomeProdutoCardapio(externalCode);
                externalCode1 = externalCode;
            }
            else
            {
                NomeProduto = $"{item.Quantity}X " + item.Name;
            }


            if (item.Observations is not null && item.Observations.Length > 0)
            {
                ObsDoItem = item.Observations;
            }

            if (ObsDoItem.Length > 80)
                ObsDoItem = ObsDoItem.Substring(0, 80);

            ClasseDeSuporte.ObsDoItem = ObsDoItem;
            ClasseDeSuporte.NomeProduto = NomeProduto;


            List<string> ListaDeObservacoes = new List<string>() { obs1, obs2, obs3, obs4, obs5, obs6, obs7, obs8, obs9, obs10, obs11, obs12, obs13, obs14 };
            foreach (var opcao in item.Additional)
            {
                var ConversorfromAdicionalToMandatory = new Mandatory()
                {
                    Sku = opcao.Sku,
                    Name = opcao.Name,
                    Group = "",
                    Value = opcao.Value,
                };

                item.Mandatory.Add(ConversorfromAdicionalToMandatory);
            }

            foreach (Mandatory opcao in item.Mandatory)
            {
                for (int i = 0; i <= ListaDeObservacoes.Count; i++)
                {
                    if (ListaDeObservacoes[i] == " ")
                    {
                        bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.Sku);

                        if (pesquisaProduto)
                        {
                            ListaDeObservacoes[i] = ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.Sku);
                            ClasseDeSuporte.Observações!.Add(ListaDeObservacoes[i]);
                        }
                        else
                        {
                            ListaDeObservacoes[i] = opcao.Name;
                            ClasseDeSuporte.Observações!.Add(ListaDeObservacoes[i]);
                        }

                        break;
                    }
                }
            }


            ClasseDeSuporte.Tamanho = "U";

            ClasseDeSuporte.ExternalCode1 = externalCode1;
            ClasseDeSuporte.ExternalCode2 = externalCode2;
            ClasseDeSuporte.ExternalCode3 = externalCode3;

            ClasseDeSuporte.Obs1 = ListaDeObservacoes[0];
            ClasseDeSuporte.Obs2 = ListaDeObservacoes[1];
            ClasseDeSuporte.Obs3 = ListaDeObservacoes[2];
            ClasseDeSuporte.Obs4 = ListaDeObservacoes[3];
            ClasseDeSuporte.Obs5 = ListaDeObservacoes[4];
            ClasseDeSuporte.Obs6 = ListaDeObservacoes[5];
            ClasseDeSuporte.Obs7 = ListaDeObservacoes[6];
            ClasseDeSuporte.Obs8 = ListaDeObservacoes[7];
            ClasseDeSuporte.Obs9 = ListaDeObservacoes[8];
            ClasseDeSuporte.Obs10 = ListaDeObservacoes[9];
            ClasseDeSuporte.Obs11 = ListaDeObservacoes[10];
            ClasseDeSuporte.Obs12 = ListaDeObservacoes[11];
            ClasseDeSuporte.Obs13 = ListaDeObservacoes[12];
            ClasseDeSuporte.Obs14 = ListaDeObservacoes[13];


            return ClasseDeSuporte;
        }
    }



    public static ClsDeSuporteParaImpressaoDosItens DefineCaracteristicasDoItemGarcomSys(ClassesGarcomSysMenu.Produto item, bool comanda = false, bool eIntegracao = false)
    {
        string? NomeProduto = "";
        ClsDeSuporteParaImpressaoDosItens ClasseDeSuporte = new ClsDeSuporteParaImpressaoDosItens();

        //Função que entra caso sejá pizza ou lanche ou porção, não mudei o nome do booleano pq já estava estruturado o cod 

        bool ePizza = item.Codigo != null && (item.Codigo2 != null || item.Codigo3 != null) ? true : false;

        string? ObsDoItem = " ";

        if (item.Observacao != null && item.Observacao != "")
        {
            if (item.Observacao.Length > 80)
            {
                ObsDoItem = item.Observacao.Substring(0, 80);
            }
            else
            {
                ObsDoItem = item.Observacao;
            }
        }

        ClasseDeSuporte.ObsDoItem = ObsDoItem;

        if (ePizza)
        {
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

            double ValorTotalDoProduto = 0;

            //aqui só define o tamanho se for pizza, se não for ele fica U de unitario
            if (item.Tamanho == "Grande")
            {
                ClasseDeSuporte.Tamanho = "G";
                ValorTotalDoProduto = item.Preco3;
            }
            else if (item.Tamanho == "Medio")
            {
                ClasseDeSuporte.Tamanho = "M";
                ValorTotalDoProduto = item.Preco2;
            }
            else if (item.Tamanho == "Pequeno")
            {
                ClasseDeSuporte.Tamanho = "P";
                ValorTotalDoProduto = item.Preco1;
            }
            else
            {
                ClasseDeSuporte.Tamanho = "U";
                ValorTotalDoProduto = item.Preco1;
            }


            ClasseDeSuporte.ExternalCode1 = item.Codigo;
            ClasseDeSuporte.ExternalCode2 = item.Codigo2 == null ? " " : item.Codigo2;
            ClasseDeSuporte.ExternalCode3 = item.Codigo3 == null ? " " : item.Codigo3; ;
            NomeProduto = "";

            List<string?> NumerosDeCodigo = new List<string?> { ClasseDeSuporte.ExternalCode1, ClasseDeSuporte.ExternalCode2, ClasseDeSuporte.ExternalCode3 };
            foreach (var cod in NumerosDeCodigo)
            {
                if (cod is not null && cod != " ")
                {
                    if (NomeProduto != "")
                        NomeProduto += "/";

                    NomeProduto += ClsDeIntegracaoSys.NomeProdutoCardapio(cod);
                }
            }


            foreach (var opcao in item.incrementos)
            {
                ValorTotalDoProduto += opcao.Valor;

                if (obs1 == " ")
                {
                    obs1 = $"{opcao.Quantidade}X {opcao.Descricao}";
                    ClasseDeSuporte.Observações!.Add(obs1);

                    continue;
                }

                if (obs2 == " ")
                {
                    obs2 = $"{opcao.Quantidade}X {opcao.Descricao}";
                    ClasseDeSuporte.Observações!.Add(obs2);

                    continue;

                }

                if (obs3 == " ")
                {
                    obs3 = $"{opcao.Quantidade}X {opcao.Descricao}";
                    ClasseDeSuporte.Observações!.Add(obs3);

                    continue;
                }

                if (obs4 == " ")
                {
                    obs4 = $"{opcao.Quantidade}X {opcao.Descricao}";
                    ClasseDeSuporte.Observações!.Add(obs4);

                    continue;
                }

                if (obs5 == " ")
                {
                    obs5 = $"{opcao.Quantidade}X {opcao.Descricao}";
                    ClasseDeSuporte.Observações!.Add(obs5);

                    continue;
                }

                if (obs6 == " ")
                {
                    obs6 = $"{opcao.Quantidade}X {opcao.Descricao}";
                    ClasseDeSuporte.Observações!.Add(obs6);

                    continue;
                }

                if (obs7 == " ")
                {
                    obs7 = $"{opcao.Quantidade}X {opcao.Descricao}";
                    ClasseDeSuporte.Observações!.Add(obs7);

                    continue;
                }

                if (obs8 == " ")
                {
                    obs8 = $"{opcao.Quantidade}X {opcao.Descricao}";
                    ClasseDeSuporte.Observações!.Add(obs8);

                    continue;
                }

                if (obs9 == " ")
                {
                    obs9 = $"{opcao.Quantidade}X {opcao.Descricao}";
                    ClasseDeSuporte.Observações!.Add(obs9);

                    continue;
                }

                if (obs10 == " ")
                {
                    obs10 = $"{opcao.Quantidade}X {opcao.Descricao}";
                    ClasseDeSuporte.Observações!.Add(obs10);

                    continue;
                }

                if (obs11 == " ")
                {
                    obs11 = $"{opcao.Quantidade}X {opcao.Descricao}";
                    ClasseDeSuporte.Observações!.Add(obs11);

                    continue;
                }

                if (obs12 == " ")
                {
                    obs12 = $"{opcao.Quantidade}X {opcao.Descricao}";
                    ClasseDeSuporte.Observações!.Add(obs12);

                    continue;
                }

                if (obs13 == " ")
                {
                    obs13 = $"{opcao.Quantidade}X {opcao.Descricao}";
                    ClasseDeSuporte.Observações!.Add(obs13);

                    continue;
                }

                if (obs14 == " ")
                {
                    obs14 = $"{opcao.Quantidade}X {opcao.Descricao}";
                    ClasseDeSuporte.Observações!.Add(obs14);

                    continue;
                }

            }


            ClasseDeSuporte.valorDoItem = Convert.ToSingle((ValorTotalDoProduto * item.Quantidade) / item.Quantidade);
            ClasseDeSuporte.valorTotalDoItem = Convert.ToSingle(ValorTotalDoProduto * item.Quantidade);

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
            string? externalCode = item.Codigo == null || item.Codigo == "" ? " " : item.Codigo;
            string? obs = item.Observacao == null || item.Observacao == "" ? " " : item.Observacao.ToString();

            string? externalCode1 = item.Codigo;
            string? externalCode2 = " ";
            string? externalCode3 = " ";

            NomeProduto = item.Descricao;
            ClasseDeSuporte.NomeProduto = NomeProduto;

            double ValorTotalDoProduto = 0;

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

            if (item.Tamanho is not null)
            {
                if (item.Tamanho == "Grande")
                {
                    ClasseDeSuporte.Tamanho = "G";
                    ValorTotalDoProduto = item.Preco3;
                }
                else if (item.Tamanho == "Medio")
                {
                    ClasseDeSuporte.Tamanho = "M";
                    ValorTotalDoProduto = item.Preco2;
                }
                else if (item.Tamanho == "Pequeno")
                {
                    ClasseDeSuporte.Tamanho = "P";
                    ValorTotalDoProduto = item.Preco1;
                }
                else
                {
                    ClasseDeSuporte.Tamanho = "U";
                    ValorTotalDoProduto = item.Preco1;
                }
            }


            foreach (var opcao in item.incrementos)
            {
                ValorTotalDoProduto += opcao.Valor * opcao.Quantidade;

                if (obs1 == " ")
                {
                    obs1 = $"{opcao.Quantidade}X {opcao.Descricao}";
                    ClasseDeSuporte.Observações!.Add(obs1);

                    continue;
                }

                if (obs2 == " ")
                {
                    obs2 = $"{opcao.Quantidade}X {opcao.Descricao}";
                    ClasseDeSuporte.Observações!.Add(obs2);

                    continue;

                }

                if (obs3 == " ")
                {
                    obs3 = $"{opcao.Quantidade}X {opcao.Descricao}";
                    ClasseDeSuporte.Observações!.Add(obs3);

                    continue;
                }

                if (obs4 == " ")
                {
                    obs4 = $"{opcao.Quantidade}X {opcao.Descricao}";
                    ClasseDeSuporte.Observações!.Add(obs4);

                    continue;
                }

                if (obs5 == " ")
                {
                    obs5 = $"{opcao.Quantidade}X {opcao.Descricao}";
                    ClasseDeSuporte.Observações!.Add(obs5);

                    continue;
                }

                if (obs6 == " ")
                {
                    obs6 = $"{opcao.Quantidade}X {opcao.Descricao}";
                    ClasseDeSuporte.Observações!.Add(obs6);

                    continue;
                }

                if (obs7 == " ")
                {
                    obs7 = $"{opcao.Quantidade}X {opcao.Descricao}";
                    ClasseDeSuporte.Observações!.Add(obs7);

                    continue;
                }

                if (obs8 == " ")
                {
                    obs8 = $"{opcao.Quantidade}X {opcao.Descricao}";
                    ClasseDeSuporte.Observações!.Add(obs8);

                    continue;
                }

                if (obs9 == " ")
                {
                    obs9 = $"{opcao.Quantidade}X {opcao.Descricao}";
                    ClasseDeSuporte.Observações!.Add(obs9);

                    continue;
                }

                if (obs10 == " ")
                {
                    obs10 = $"{opcao.Quantidade}X {opcao.Descricao}";
                    ClasseDeSuporte.Observações!.Add(obs10);

                    continue;
                }

                if (obs11 == " ")
                {
                    obs11 = $"{opcao.Quantidade}X {opcao.Descricao}";
                    ClasseDeSuporte.Observações!.Add(obs11);

                    continue;
                }

                if (obs12 == " ")
                {
                    obs12 = $"{opcao.Quantidade}X {opcao.Descricao}";
                    ClasseDeSuporte.Observações!.Add(obs12);

                    continue;
                }

                if (obs13 == " ")
                {
                    obs13 = $"{opcao.Quantidade}X {opcao.Descricao}";
                    ClasseDeSuporte.Observações!.Add(obs13);

                    continue;
                }

                if (obs14 == " ")
                {
                    obs14 = $"{opcao.Quantidade}X {opcao.Descricao}";
                    ClasseDeSuporte.Observações!.Add(obs14);

                    continue;
                }

            }


            ClasseDeSuporte.valorDoItem = Convert.ToSingle((ValorTotalDoProduto * item.Quantidade) / item.Quantidade);
            ClasseDeSuporte.valorTotalDoItem = Convert.ToSingle(ValorTotalDoProduto * item.Quantidade);

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

    public static ClsDeSuporteParaImpressaoDosItens DefineCaracteristicasDoItemCCM(SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoCCM.Item item, bool comanda = false)
    {
        string? NomeProduto = "";
        ClsDeSuporteParaImpressaoDosItens ClasseDeSuporte = new ClsDeSuporteParaImpressaoDosItens();

        //Função que entra caso sejá pizza ou lanche ou porção, não mudei o nome do booleano pq já estava estruturado o cod 

        bool ePizza = item.CodPdvGrupo == "G" || item.CodPdvGrupo == "M" || item.CodPdvGrupo == "P" || item.CodPdvGrupo == "B" || item.CodPdvGrupo == "LAN" || item.CodPdvGrupo == "PRC" ? true : false;

        string? ObsDoItem = " ";


        if (ePizza)
        {
            // string obs = item.observations == null || item.observations == "" ? " " : item.observations.ToString();
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

            if (item.Parte.Count > 0)
            {
                foreach (var option in item.Parte)
                {
                    bool eIncremento = option.CodPdvItem.Length == 3 ? true : false;

                    if (!option.CodPdvItem.Contains("m") && !eIncremento && ePizza1 == null)
                    {
                        ePizza1 = option.CodPdvItem == "" ? " " : option.CodPdvItem;
                        bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(option.CodPdvItem);

                        if (pesquisaProduto)
                        {
                            NomeProduto = $"{item.Quantidade}X " + ClsDeIntegracaoSys.NomeProdutoCardapio(option.CodPdvItem);
                            externalCode1 = option.CodPdvItem;
                        }
                        else
                        {
                            NomeProduto += $"{item.Quantidade}X " + item.NomeItem;
                        }

                        if (option.ObsParte is not null && option.ObsParte.Length > 0)
                        {
                            ClasseDeSuporte.Observações!.Add($"Obs Parte 1 : {option.ObsParte}");
                            ObsDoItem += option.ObsParte;
                        }

                        continue;
                    }

                    if (!option.CodPdvItem.Contains("m") && !eIncremento && ePizza2 == null)
                    {
                        ePizza2 = option.CodPdvItem == "" ? " " : option.CodPdvItem;
                        bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(option.CodPdvItem);

                        if (pesquisaProduto)
                        {
                            NomeProduto += " / " + ClsDeIntegracaoSys.NomeProdutoCardapio(option.CodPdvItem);
                            externalCode2 = option.CodPdvItem;
                        }

                        if (option.ObsParte is not null && option.ObsParte.Length > 0)
                        {
                            ClasseDeSuporte.Observações!.Add($"Obs Parte 2 : {option.ObsParte}");
                            ObsDoItem += $" ;{option.ObsParte}";
                        }

                        continue;
                    }

                    if (!option.CodPdvItem.Contains("m") && !eIncremento && ePizza3 == null)
                    {
                        ePizza3 = option.CodPdvItem == "" ? " " : option.CodPdvItem;
                        bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(option.CodPdvItem);

                        if (pesquisaProduto)
                        {
                            NomeProduto += " / " + ClsDeIntegracaoSys.NomeProdutoCardapio(option.CodPdvItem);
                            externalCode3 = option.CodPdvItem;
                        }

                        if (option.ObsParte is not null && option.ObsParte.Length > 0)
                        {
                            ClasseDeSuporte.Observações!.Add($"Obs Parte 3 : {option.ObsParte}");
                            ObsDoItem += $" ;{option.ObsParte}";
                        }

                        continue;
                    }

                }
            }
            else
            {
                bool existeProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(item.CodPdv);

                if (existeProduto)
                {
                    NomeProduto = $"{item.Quantidade}X " + ClsDeIntegracaoSys.NomeProdutoCardapio(item.CodPdv);
                    externalCode1 = item.CodPdv;
                }
                else
                {
                    NomeProduto = $"{item.Quantidade}X " + item.NomeItem;
                }

                if (item.ObsItem is not null && item.ObsItem != "")
                {
                    ObsDoItem = item.ObsItem;
                }

            }

            if (ObsDoItem.Length > 80)
                ObsDoItem = ObsDoItem.Substring(0, 80);

            ClasseDeSuporte.ObsDoItem = ObsDoItem;


            //aqui só define o tamanho se for pizza, se não for ele fica U de uitario

            ClasseDeSuporte.Tamanho = item.CodPdvGrupo == "G" || item.CodPdvGrupo == "M" || item.CodPdvGrupo == "P" || item.CodPdvGrupo == "B" ? item.CodPdvGrupo : "U";
            ClasseDeSuporte.ExternalCode1 = externalCode1;
            ClasseDeSuporte.ExternalCode2 = externalCode2;
            ClasseDeSuporte.ExternalCode3 = externalCode3;

            foreach (var opcao in item.Adicionais)
            {
                bool eIncremento = opcao.CodPdv.Length == 3 ? true : false;

                if (obs1 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.CodPdv);

                    if (pesquisaProduto)
                    {
                        obs1 = $"{opcao.Quantidade}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.CodPdv)}";
                        ClasseDeSuporte.Observações.Add(obs1);
                    }
                    else
                    {
                        string precoProduto = $"- {opcao.ValorUnit.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs1 = $"{opcao.Quantidade}X {opcao.Descricao} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs1);
                    }

                    continue;
                }

                if (obs2 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.CodPdv);

                    if (pesquisaProduto)
                    {
                        obs2 = $"{opcao.Quantidade}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.CodPdv)}";
                        ClasseDeSuporte.Observações.Add(obs2);
                    }
                    else
                    {
                        string precoProduto = $"- {opcao.ValorUnit.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs2 = $"{opcao.Quantidade}X {opcao.Descricao} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs2);
                    }

                    continue;
                }

                if (obs3 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.CodPdv);

                    if (pesquisaProduto)
                    {
                        obs3 = $"{opcao.Quantidade}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.CodPdv)}";
                        ClasseDeSuporte.Observações.Add(obs3);
                    }
                    else
                    {
                        string precoProduto = $"- {opcao.ValorUnit.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs3 = $"{opcao.Quantidade}X {opcao.Descricao} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs3);
                    }

                    continue;
                }

                if (obs4 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.CodPdv);

                    if (pesquisaProduto)
                    {
                        obs4 = $"{opcao.Quantidade}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.CodPdv)}";
                        ClasseDeSuporte.Observações.Add(obs4);
                    }
                    else
                    {
                        string precoProduto = $"- {opcao.ValorUnit.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs4 = $"{opcao.Quantidade}X {opcao.Descricao} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs4);
                    }

                    continue;
                }

                if (obs5 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.CodPdv);

                    if (pesquisaProduto)
                    {
                        obs5 = $"{opcao.Quantidade}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.CodPdv)}";
                        ClasseDeSuporte.Observações.Add(obs5);
                    }
                    else
                    {
                        string precoProduto = $"- {opcao.ValorUnit.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs5 = $"{opcao.Quantidade}X {opcao.Descricao} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs5);
                    }

                    continue;
                }

                if (obs6 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.CodPdv);

                    if (pesquisaProduto)
                    {
                        obs6 = $"{opcao.Quantidade}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.CodPdv)}";
                        ClasseDeSuporte.Observações.Add(obs6);
                    }
                    else
                    {
                        string precoProduto = $"- {opcao.ValorUnit.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs6 = $"{opcao.Quantidade}X {opcao.Descricao} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs6);
                    }

                    continue;
                }

                if (obs7 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.CodPdv);

                    if (pesquisaProduto)
                    {
                        obs7 = $"{opcao.Quantidade}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.CodPdv)}";
                        ClasseDeSuporte.Observações.Add(obs7);
                    }
                    else
                    {
                        string precoProduto = $"- {opcao.ValorUnit.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs7 = $"{opcao.Quantidade}X {opcao.Descricao} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs7);
                    }

                    continue;
                }

                if (obs8 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.CodPdv);

                    if (pesquisaProduto)
                    {
                        obs8 = $"{opcao.Quantidade}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.CodPdv)}";
                        ClasseDeSuporte.Observações.Add(obs8);
                    }
                    else
                    {
                        string precoProduto = $"- {opcao.ValorUnit.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs8 = $"{opcao.Quantidade}X {opcao.Descricao} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs8);
                    }

                    continue;
                }

                if (obs9 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.CodPdv);

                    if (pesquisaProduto)
                    {
                        obs9 = $"{opcao.Quantidade}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.CodPdv)}";
                        ClasseDeSuporte.Observações.Add(obs9);
                    }
                    else
                    {
                        string precoProduto = $"- {opcao.ValorUnit.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs9 = $"{opcao.Quantidade}X {opcao.Descricao} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs9);
                    }

                    continue;
                }

                if (obs10 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.CodPdv);

                    if (pesquisaProduto)
                    {
                        obs10 = $"{opcao.Quantidade}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.CodPdv)}";
                        ClasseDeSuporte.Observações.Add(obs10);
                    }
                    else
                    {
                        string precoProduto = $"- {opcao.ValorUnit.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs10 = $"{opcao.Quantidade}X {opcao.Descricao} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs10);
                    }

                    continue;
                }

                if (obs11 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.CodPdv);

                    if (pesquisaProduto)
                    {
                        obs11 = $"{opcao.Quantidade}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.CodPdv)}";
                        ClasseDeSuporte.Observações.Add(obs11);
                    }
                    else
                    {
                        string precoProduto = $"- {opcao.ValorUnit.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs11 = $"{opcao.Quantidade}X {opcao.Descricao} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs11);
                    }

                    continue;
                }

                if (obs12 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.CodPdv);

                    if (pesquisaProduto)
                    {
                        obs12 = $"{opcao.Quantidade}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.CodPdv)}";
                        ClasseDeSuporte.Observações.Add(obs12);
                    }
                    else
                    {
                        string precoProduto = $"- {opcao.ValorUnit.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs12 = $"{opcao.Quantidade}X {opcao.Descricao} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs12);
                    }

                    continue;
                }

                if (obs13 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.CodPdv);

                    if (pesquisaProduto)
                    {
                        obs13 = $"{opcao.Quantidade}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.CodPdv)}";
                        ClasseDeSuporte.Observações.Add(obs13);
                    }
                    else
                    {
                        string precoProduto = $"- {opcao.ValorUnit.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs13 = $"{opcao.Quantidade}X {opcao.Descricao} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs13);
                    }

                    continue;
                }

                if (obs14 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.CodPdv);

                    if (pesquisaProduto)
                    {
                        obs14 = $"{opcao.Quantidade}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.CodPdv)}";
                        ClasseDeSuporte.Observações.Add(obs14);
                    }
                    else
                    {
                        string precoProduto = $"- {opcao.ValorUnit.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs14 = $"{opcao.Quantidade}X {opcao.Descricao} {precoProduto}";
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
            string? externalCode = item.CodPdv == null || item.CodPdv == "" ? " " : item.CodPdv;
            string? obs = " ";//item.observations == null || item.observations == "" ? " " : item.observations.ToString();

            string? externalCode1 = " ";
            string? externalCode2 = " ";
            string? externalCode3 = " ";

            bool existeProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(externalCode);

            if (existeProduto)
            {
                NomeProduto = $"{item.Quantidade}X " + ClsDeIntegracaoSys.NomeProdutoCardapio(externalCode);
                externalCode1 = externalCode;
            }
            else
            {
                NomeProduto = $"{item.Quantidade}X " + item.NomeItem;
            }


            if (item.ObsItem is not null && item.ObsItem.Length > 0)
            {
                ObsDoItem = item.ObsItem;
            }

            if (ObsDoItem.Length > 80)
                ObsDoItem = ObsDoItem.Substring(0, 80);

            ClasseDeSuporte.ObsDoItem = ObsDoItem;
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


            foreach (var opcao in item.Adicionais)
            {
                if (obs1 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.CodPdv);

                    if (pesquisaProduto)
                    {
                        obs1 = $"{opcao.Quantidade}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.CodPdv)}";
                        ClasseDeSuporte.Observações.Add(obs1);
                    }
                    else
                    {
                        string precoProduto = $"- {opcao.ValorUnit.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs1 = $"{opcao.Quantidade}X {opcao.Descricao} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs1);
                    }

                    continue;
                }

                if (obs2 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.CodPdv);

                    if (pesquisaProduto)
                    {
                        obs2 = $"{opcao.Quantidade}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.CodPdv)}";
                        ClasseDeSuporte.Observações.Add(obs2);
                    }
                    else
                    {
                        string precoProduto = $"- {opcao.ValorUnit.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs2 = $"{opcao.Quantidade}X {opcao.Descricao} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs2);
                    }

                    continue;
                }

                if (obs3 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.CodPdv);

                    if (pesquisaProduto)
                    {
                        obs3 = $"{opcao.Quantidade}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.CodPdv)}";
                        ClasseDeSuporte.Observações.Add(obs3);
                    }
                    else
                    {
                        string precoProduto = $"- {opcao.ValorUnit.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs3 = $"{opcao.Quantidade}X {opcao.Descricao} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs3);
                    }

                    continue;
                }

                if (obs4 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.CodPdv);

                    if (pesquisaProduto)
                    {
                        obs4 = $"{opcao.Quantidade}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.CodPdv)}";
                        ClasseDeSuporte.Observações.Add(obs4);
                    }
                    else
                    {
                        string precoProduto = $"- {opcao.ValorUnit.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs4 = $"{opcao.Quantidade}X {opcao.Descricao} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs4);
                    }

                    continue;
                }

                if (obs5 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.CodPdv);

                    if (pesquisaProduto)
                    {
                        obs5 = $"{opcao.Quantidade}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.CodPdv)}";
                        ClasseDeSuporte.Observações.Add(obs5);
                    }
                    else
                    {
                        string precoProduto = $"- {opcao.ValorUnit.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs5 = $"{opcao.Quantidade}X {opcao.Descricao} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs5);
                    }

                    continue;
                }

                if (obs6 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.CodPdv);

                    if (pesquisaProduto)
                    {
                        obs6 = $"{opcao.Quantidade}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.CodPdv)}";
                        ClasseDeSuporte.Observações.Add(obs6);
                    }
                    else
                    {
                        string precoProduto = $"- {opcao.ValorUnit.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs6 = $"{opcao.Quantidade}X {opcao.Descricao} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs6);
                    }

                    continue;
                }

                if (obs7 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.CodPdv);

                    if (pesquisaProduto)
                    {
                        obs7 = $"{opcao.Quantidade}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.CodPdv)}";
                        ClasseDeSuporte.Observações.Add(obs7);
                    }
                    else
                    {
                        string precoProduto = $"- {opcao.ValorUnit.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs7 = $"{opcao.Quantidade}X {opcao.Descricao} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs7);
                    }

                    continue;
                }

                if (obs8 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.CodPdv);

                    if (pesquisaProduto)
                    {
                        obs8 = $"{opcao.Quantidade}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.CodPdv)}";
                        ClasseDeSuporte.Observações.Add(obs8);
                    }
                    else
                    {
                        string precoProduto = $"- {opcao.ValorUnit.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs8 = $"{opcao.Quantidade}X {opcao.Descricao} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs8);
                    }

                    continue;
                }

                if (obs9 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.CodPdv);

                    if (pesquisaProduto)
                    {
                        obs9 = $"{opcao.Quantidade}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.CodPdv)}";
                        ClasseDeSuporte.Observações.Add(obs9);
                    }
                    else
                    {
                        string precoProduto = $"- {opcao.ValorUnit.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs9 = $"{opcao.Quantidade}X {opcao.Descricao} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs9);
                    }

                    continue;
                }

                if (obs10 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.CodPdv);

                    if (pesquisaProduto)
                    {
                        obs9 = $"{opcao.Quantidade}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.CodPdv)}";
                        ClasseDeSuporte.Observações.Add(obs10);
                    }
                    else
                    {
                        string precoProduto = $"- {opcao.ValorUnit.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs9 = $"{opcao.Quantidade}X {opcao.Descricao} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs10);
                    }

                    continue;
                }

                if (obs11 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.CodPdv);

                    if (pesquisaProduto)
                    {
                        obs11 = $"{opcao.Quantidade}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.CodPdv)}";
                        ClasseDeSuporte.Observações.Add(obs11);
                    }
                    else
                    {
                        string precoProduto = $"- {opcao.ValorUnit.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs11 = $"{opcao.Quantidade}X {opcao.Descricao} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs11);
                    }

                    continue;
                }

                if (obs12 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.CodPdv);

                    if (pesquisaProduto)
                    {
                        obs12 = $"{opcao.Quantidade}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.CodPdv)}";
                        ClasseDeSuporte.Observações.Add(obs12);
                    }
                    else
                    {
                        string precoProduto = $"- {opcao.ValorUnit.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs12 = $"{opcao.Quantidade}X {opcao.Descricao} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs12);
                    }

                    continue;
                }

                if (obs13 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.CodPdv);

                    if (pesquisaProduto)
                    {
                        obs13 = $"{opcao.Quantidade}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.CodPdv)}";
                        ClasseDeSuporte.Observações.Add(obs13);
                    }
                    else
                    {
                        string precoProduto = $"- {opcao.ValorUnit.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs13 = $"{opcao.Quantidade}X {opcao.Descricao} {precoProduto}";
                        ClasseDeSuporte.Observações.Add(obs13);
                    }

                    continue;
                }

                if (obs14 == " ")
                {
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(opcao.CodPdv);

                    if (pesquisaProduto)
                    {
                        obs14 = $"{opcao.Quantidade}X {ClsDeIntegracaoSys.NomeProdutoCardapio(opcao.CodPdv)}";
                        ClasseDeSuporte.Observações.Add(obs14);
                    }
                    else
                    {
                        string precoProduto = $"- {opcao.ValorUnit.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs14 = $"{opcao.Quantidade}X {opcao.Descricao} {precoProduto}";
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


            string? externalCode1 = " ";
            string? externalCode2 = " ";
            string? externalCode3 = " ";

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

            int quantidadeDePizzas = item.options.Count(x => x.externalCode.Length > 3 || !x.externalCode.Contains("m"));

            foreach (var option in item.options)
            {
                string? montaQuantidaDePizzas = "";

                if (quantidadeDePizzas > 1)
                    montaQuantidaDePizzas = $"1/{quantidadeDePizzas}-";

                if (!option.externalCode.Contains("m") && ePizza1 == null)
                {
                    ePizza1 = option.externalCode == "" ? " " : option.externalCode;
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(option.externalCode);

                    if (pesquisaProduto)
                    {
                        NomeProduto += montaQuantidaDePizzas + ClsDeIntegracaoSys.NomeProdutoCardapio(option.externalCode);
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
                        NomeProduto += " / " + montaQuantidaDePizzas + ClsDeIntegracaoSys.NomeProdutoCardapio(option.externalCode);
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
                        NomeProduto += " / " + montaQuantidaDePizzas + ClsDeIntegracaoSys.NomeProdutoCardapio(option.externalCode);
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
                        string precoProduto = $"- {((opcao.price) * (item.quantity)).ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs1 = $"{opcao.quantity * item.quantity}X  {opcao.name} {precoProduto}";
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
                        string precoProduto = $"- {((opcao.price) * (item.quantity)).ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs2 = $"{opcao.quantity * item.quantity}X  {opcao.name} {precoProduto}";
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
                        string precoProduto = $"- {((opcao.price) * (item.quantity)).ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs3 = $"{opcao.quantity * item.quantity}X  {opcao.name} {precoProduto}";
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
                        string precoProduto = $"- {((opcao.price) * (item.quantity)).ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs4 = $"{opcao.quantity * item.quantity}X  {opcao.name} {precoProduto}";
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
                        string precoProduto = $"- {((opcao.price) * (item.quantity)).ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs5 = $"{opcao.quantity * item.quantity}X  {opcao.name} {precoProduto}";
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
                        string precoProduto = $"- {((opcao.price) * (item.quantity)).ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs6 = $"{opcao.quantity * item.quantity}X  {opcao.name} {precoProduto}";
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
                        string precoProduto = $"- {((opcao.price) * (item.quantity)).ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs7 = $"{opcao.quantity * item.quantity}X  {opcao.name} {precoProduto}";
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
                        string precoProduto = $"- {((opcao.price) * (item.quantity)).ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs8 = $"{opcao.quantity * item.quantity}X  {opcao.name} {precoProduto}";
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
                        string precoProduto = $"- {((opcao.price) * (item.quantity)).ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs9 = $"{opcao.quantity * item.quantity}X  {opcao.name} {precoProduto}";
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
                        string precoProduto = $"- {((opcao.price) * (item.quantity)).ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs10 = $"{opcao.quantity * item.quantity}X  {opcao.name} {precoProduto}";
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
                        string precoProduto = $"- {((opcao.price) * (item.quantity)).ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs11 = $"{opcao.quantity * item.quantity}X  {opcao.name} {precoProduto}";
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
                        string precoProduto = $"- {((opcao.price) * (item.quantity)).ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs12 = $"{opcao.quantity * item.quantity}X  {opcao.name} {precoProduto}";
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
                        string precoProduto = $"- {((opcao.price) * (item.quantity)).ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs13 = $"{opcao.quantity * item.quantity}X  {opcao.name} {precoProduto}";
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
                        string precoProduto = $"- {((opcao.price) * (item.quantity)).ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs14 = $"{opcao.quantity * item.quantity}X  {opcao.name} {precoProduto}";
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
                        string precoProduto = $"- {((opcao.price) * (item.quantity)).ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs1 = $"{opcao.quantity * item.quantity}X  {opcao.name} {precoProduto}";
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
                        string precoProduto = $"- {((opcao.price) * (item.quantity)).ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs2 = $"{opcao.quantity * item.quantity}X  {opcao.name} {precoProduto}";
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
                        string precoProduto = $"- {((opcao.price) * (item.quantity)).ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs3 = $"{opcao.quantity * item.quantity}X  {opcao.name} {precoProduto}";
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
                        string precoProduto = $"- {((opcao.price) * (item.quantity)).ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs4 = $"{opcao.quantity * item.quantity}X  {opcao.name} {precoProduto}";
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
                        string precoProduto = $"- {((opcao.price) * (item.quantity)).ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs5 = $"{opcao.quantity * item.quantity}X  {opcao.name} {precoProduto}";
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
                        string precoProduto = $"- {((opcao.price) * (item.quantity)).ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs6 = $"{opcao.quantity * item.quantity}X  {opcao.name} {precoProduto}";
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
                        string precoProduto = $"- {((opcao.price) * (item.quantity)).ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs7 = $"{opcao.quantity * item.quantity}X  {opcao.name} {precoProduto}";
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
                        string precoProduto = $"- {((opcao.price) * (item.quantity)).ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs8 = $"{opcao.quantity * item.quantity}X  {opcao.name} {precoProduto}";
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
                        string precoProduto = $"- {((opcao.price) * (item.quantity)).ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs9 = $"{opcao.quantity * item.quantity}X  {opcao.name} {precoProduto}";
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
                        string precoProduto = $"- {((opcao.price) * (item.quantity)).ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs10 = $"{opcao.quantity * item.quantity}X  {opcao.name} {precoProduto}";
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
                        string precoProduto = $"- {((opcao.price) * (item.quantity)).ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs11 = $"{opcao.quantity * item.quantity}X  {opcao.name} {precoProduto}";
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
                        string precoProduto = $"- {((opcao.price) * (item.quantity)).ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs12 = $"{opcao.quantity * item.quantity}X  {opcao.name} {precoProduto}";
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
                        string precoProduto = $"- {((opcao.price) * (item.quantity)).ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs13 = $"{opcao.quantity * item.quantity}X  {opcao.name} {precoProduto}";
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
                        string precoProduto = $"- {((opcao.price) * (item.quantity)).ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs14 = $"{opcao.quantity * item.quantity}X  {opcao.name} {precoProduto}";
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

        bool ePizza = item.ExternalCode == "G" || item.ExternalCode == "M" || item.ExternalCode == "P" || item.ExternalCode!.Contains("TM", StringComparison.OrdinalIgnoreCase) || item.ExternalCode!.Contains("PRC", StringComparison.OrdinalIgnoreCase) || item.ExternalCode!.Contains("LAN", StringComparison.OrdinalIgnoreCase) || item.ExternalCode!.Contains("BB", StringComparison.OrdinalIgnoreCase) ? true : false;

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
                if (!option.ExternalCode.Contains("m", StringComparison.OrdinalIgnoreCase) || !option.ExternalCode.Contains("ig", StringComparison.OrdinalIgnoreCase) && ePizza1 == null)
                {
                    ePizza1 = option.ExternalCode == "" ? " " : option.ExternalCode;
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(option.ExternalCode);

                    if (pesquisaProduto)
                    {
                        NomeProduto = $"{item.Quantity}X " + ClsDeIntegracaoSys.NomeProdutoCardapio(option.ExternalCode);
                        externalCode1 = option.ExternalCode;
                    }
                    else
                    {
                        NomeProduto = $"{item.Quantity}X " + option.Name;
                    }
                    continue;
                }

                if (!option.ExternalCode.Contains("m", StringComparison.OrdinalIgnoreCase) || !option.ExternalCode.Contains("ig", StringComparison.OrdinalIgnoreCase) && ePizza2 == null)
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

                if (!option.ExternalCode.Contains("m", StringComparison.OrdinalIgnoreCase) || !option.ExternalCode.Contains("ig", StringComparison.OrdinalIgnoreCase) && ePizza3 == null)
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

            if (item.ExternalCode!.Length > 1)
                item.ExternalCode = "U";

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
                NomeProduto = $"{item.Quantity}X " + ClsDeIntegracaoSys.NomeProdutoCardapio(externalCode);
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

    public static ClsDeSuporteParaImpressaoDosItens DefineCaracteristicasDoItemAnotaAi(ItemAnotaAi item, bool comanda = false, bool eIntegracao = false)
    {
        string? NomeProduto = "";
        ClsDeSuporteParaImpressaoDosItens ClasseDeSuporte = new ClsDeSuporteParaImpressaoDosItens();

        //Função que entra caso sejá pizza ou lanche ou porção, não mudei o nome do booleano pq já estava estruturado o cod 

        bool ePizza = item.externalCode == "G" || item.externalCode == "M" || item.externalCode == "P" || item.externalCode == "B" || item.externalCode == "LAN" || item.externalCode == "PRC" ? true : false;

        string? ObsDoItem = " ";

        if (item.observation != null && item.observation.Length > 0)
        {
            if (item.observation.Length > 80)
            {
                ObsDoItem = item.observation.Substring(0, 80);
            }
            else
            {
                ObsDoItem = item.observation;
            }
        }

        ClasseDeSuporte.ObsDoItem = ObsDoItem;

        if (ePizza)
        {
            string obs = item.observation == null || item.observation == "" ? " " : item.observation.ToString();
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

            foreach (var option in item.SubItens)
            {
                bool eIncremento = option.externalCode.Length == 3 ? true : false;

                if (!option.externalCode.Contains("m") && !eIncremento && ePizza1 == null)
                {
                    ePizza1 = option.externalCode == "" ? " " : option.externalCode;
                    bool pesquisaProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(option.externalCode);

                    if (pesquisaProduto)
                    {
                        if (!eIntegracao)
                        {
                            NomeProduto = $"{item.quantity}X " + ClsDeIntegracaoSys.NomeProdutoCardapio(option.externalCode);
                            externalCode1 = option.externalCode;
                        }
                        else
                        {
                            NomeProduto = ClsDeIntegracaoSys.NomeProdutoCardapio(option.externalCode);
                            externalCode1 = option.externalCode;
                        }
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

            foreach (var opcao in item.SubItens)
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
                        string precoProduto = $"- {opcao.TotalPrice.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs1 = $"{opcao.quantity}X {opcao.name} {precoProduto}";
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
                        string precoProduto = $"- {opcao.TotalPrice.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs2 = $"{opcao.quantity}X {opcao.name} {precoProduto}";
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
                        string precoProduto = $"- {opcao.TotalPrice.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs3 = $"{opcao.quantity}X {opcao.name} {precoProduto}";
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
                        string precoProduto = $"-  {opcao.TotalPrice.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs4 = $"{opcao.quantity}X {opcao.name} {precoProduto}";
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
                        string precoProduto = $"- {opcao.TotalPrice.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs5 = $"{opcao.quantity}X {opcao.name} {precoProduto}";
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
                        string precoProduto = $"- {opcao.TotalPrice.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs6 = $"{opcao.quantity}X {opcao.name} {precoProduto}";
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
                        string precoProduto = $"- {opcao.TotalPrice.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs7 = $"{opcao.quantity}X {opcao.name} {precoProduto}";
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
                        string precoProduto = $"- {opcao.TotalPrice.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs8 = $"{opcao.quantity}X {opcao.name} {precoProduto}";
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
                        string precoProduto = $"- {opcao.TotalPrice.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs9 = $"{opcao.quantity}X {opcao.name} {precoProduto}";
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
                        string precoProduto = $"- {opcao.TotalPrice.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs10 = $"{opcao.quantity}X {opcao.name} {precoProduto}";
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
                        string precoProduto = $"- {opcao.TotalPrice.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs11 = $"{opcao.quantity}X {opcao.name} {precoProduto}";
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
                        string precoProduto = $"- {opcao.TotalPrice.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs12 = $"{opcao.quantity}X {opcao.name} {precoProduto}";
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
                        string precoProduto = $"- {opcao.TotalPrice.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs13 = $"{opcao.quantity}X {opcao.name} {precoProduto}";
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
                        string precoProduto = $"- {opcao.TotalPrice.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs14 = $"{opcao.quantity}X {opcao.name} {precoProduto}";
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
            string? obs = item.observation == null || item.observation == "" ? " " : item.observation.ToString();

            string? externalCode1 = " ";
            string? externalCode2 = " ";
            string? externalCode3 = " ";

            bool existeProduto = ClsDeIntegracaoSys.PesquisaCodCardapio(externalCode);

            if (existeProduto)
            {
                if (!eIntegracao)
                {
                    NomeProduto = $"{item.quantity}X " + ClsDeIntegracaoSys.NomeProdutoCardapio(externalCode);
                    externalCode1 = externalCode;
                }
                else
                {
                    NomeProduto = ClsDeIntegracaoSys.NomeProdutoCardapio(externalCode);
                    externalCode1 = externalCode;
                }
            }
            else
            {
                NomeProduto = $"{item.quantity}X " + item.Name;
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


            foreach (var opcao in item.SubItens)
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
                        string precoProduto = $"- {opcao.TotalPrice.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs1 = $"{opcao.quantity}X {opcao.name} {precoProduto}";
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
                        string precoProduto = $"- {opcao.TotalPrice.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs2 = $"{opcao.quantity}X {opcao.name} {precoProduto}";
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
                        string precoProduto = $"- {opcao.TotalPrice.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs3 = $"{opcao.quantity}X {opcao.name} {precoProduto}";
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
                        string precoProduto = $"- {opcao.TotalPrice.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs4 = $"{opcao.quantity}X {opcao.name} {precoProduto}";
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
                        string precoProduto = $"- {opcao.TotalPrice.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs5 = $"{opcao.quantity}X {opcao.name} {precoProduto}";
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
                        string precoProduto = $"- {opcao.TotalPrice.ToString("c")}";
                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs6 = $"{opcao.quantity}X {opcao.name} {precoProduto}";
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
                        string precoProduto = $"- {opcao.TotalPrice.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs7 = $"{opcao.quantity}X {opcao.name} {precoProduto}";
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
                        string precoProduto = $"- {opcao.TotalPrice.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs8 = $"{opcao.quantity}X {opcao.name} {precoProduto}";
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
                        string precoProduto = $"- {opcao.TotalPrice.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs9 = $"{opcao.quantity}X {opcao.name} {precoProduto}";
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
                        string precoProduto = $"- {opcao.TotalPrice.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs10 = $"{opcao.quantity}X {opcao.name} {precoProduto}";
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
                        string precoProduto = $"- {opcao.TotalPrice.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs11 = $"{opcao.quantity}X {opcao.name} {precoProduto}";
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
                        string precoProduto = $"- {opcao.TotalPrice.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs12 = $"{opcao.quantity}X {opcao.name} {precoProduto}";
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
                        string precoProduto = $"- {opcao.TotalPrice.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs13 = $"{opcao.quantity}X {opcao.name} {precoProduto}";
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
                        string precoProduto = $"- {opcao.TotalPrice.ToString("c")}";

                        if (comanda || precoProduto == "- R$ 0,00")
                        {
                            precoProduto = " ";
                        }

                        obs14 = $"{opcao.quantity}X {opcao.name} {precoProduto}";
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
    public async Task<Sequencia> PesquisaClientesNoCadastro(string? telefone)
    {
        Sequencia sequencia = new Sequencia();
        try
        {
            using (ApplicationDbContext dbPostgres = await _Context.GetContextoAsync())
            {
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
                        }

                    }
                }
            }

            return sequencia;
        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
            MessageBox.Show(ex.ToString(), "Erro Ao listar Cliente Cadastrado");
        }
        return sequencia;
    }

    public async Task<Sequencia> PesquisaEnderecoDeEntrega(string? numConta, string? metodo, string? empresaId = "DelMatchId")
    {
        Sequencia sequencia = new Sequencia();
        try
        {
            using (ApplicationDbContext dbPostgres = await _Context.GetContextoAsync())
            {
                ParametrosDoSistema? opcSistema = dbPostgres.parametrosdosistema.ToList().FirstOrDefault();

                string? caminhoBancoAccess = opcSistema.CaminhodoBanco;
                string? SqlSelectIntoCadastros = "";

                if (metodo == "ENVIARPEDIDO")
                {
                    SqlSelectIntoCadastros = $"SELECT * FROM Sequencia WHERE CONTA = @NUMCONTA AND {empresaId} IS NULL";
                }

                if (metodo == "GETPEDIDO")
                {
                    SqlSelectIntoCadastros = $"SELECT * FROM Sequencia WHERE CONTA = @NUMCONTA AND {empresaId} IS NOT NULL";
                }

                using (OleDbConnection connection = new OleDbConnection(caminhoBancoAccess))
                {
                    connection.Open();

                    using (OleDbCommand selectCommand = new OleDbCommand(SqlSelectIntoCadastros, connection))
                    {
                        selectCommand.Parameters.AddWithValue("@NUMCONTA", numConta);

                        // Executar a consulta SELECT
                        using (OleDbDataReader reader = selectCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                sequencia.Customer.Name = reader["CONTATO"].ToString();
                                sequencia.Customer.Phone = reader["TELEFONE"].ToString();
                                sequencia.Customer.TaxPayerIdentificationNumber = "CPF"; //falta terminar

                                sequencia.DeliveryAddress.FormattedAddress = reader["ENDENTREGA"].ToString();
                                sequencia.DeliveryAddress.Country = "BR";
                                sequencia.DeliveryAddress.State = "SP";
                                sequencia.DeliveryAddress.City = opcSistema.Cidade;
                                sequencia.DeliveryAddress.Neighborhood = reader["BAIENTREGA"].ToString();
                                sequencia.DeliveryAddress.StreetName = reader["ENDENTREGA"].ToString();
                                sequencia.DeliveryAddress.StreetNumber = "";
                                sequencia.DeliveryAddress.PostalCode = "";
                                sequencia.DeliveryAddress.Complement = reader["REFENTREGA"].ToString();

                                sequencia.DeliveryAddress.Coordinates.Latitude = 0;
                                sequencia.DeliveryAddress.Coordinates.Longitude = 0;
                            }
                        }

                    }
                }
            }
            return sequencia;
        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
            MessageBox.Show(ex.ToString(), "Erro Ao listar Cliente Cadastrado");
        }
        return sequencia;
    }

    public async Task<List<Sequencia>> ListarPedidosAbertos(string? CodEntregador = "99", string? empresaId = "DelMatchId") //método que serve para enviarmos um pedido
    {
        List<Sequencia> sequencias = new List<Sequencia>();
        try
        {
            using (ApplicationDbContext dbPostgres = await _Context.GetContextoAsync())
            {
                ParametrosDoSistema? opcSistema = dbPostgres.parametrosdosistema.FirstOrDefault();

                string? caminhoBancoAccess = opcSistema.CaminhodoBanco;

                string entregador = "99";
                string SqlSelectIntoCadastros = $"SELECT * FROM Sequencia WHERE TRIM(ENTREGADOR) = @ENTREGADOR AND {empresaId} IS NULL";

                using (OleDbConnection connection = new OleDbConnection(caminhoBancoAccess))
                {
                    connection.Open();

                    using (OleDbCommand selectCommand = new OleDbCommand(SqlSelectIntoCadastros, connection))
                    {
                        selectCommand.Parameters.AddWithValue("@ENTREGADOR", CodEntregador);

                        // Executar a consulta SELECT
                        using (OleDbDataReader reader = selectCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string telefone = reader["TELEFONE"].ToString();
                                Sequencia sequencia = await PesquisaEnderecoDeEntrega(reader["CONTA"].ToString(), "ENVIARPEDIDO"); //PesquisaClientesNoCadastro(telefone.Trim());

                                if (sequencia.DeliveryAddress.FormattedAddress == null || sequencia.DeliveryAddress.FormattedAddress.Length <= 3)
                                {
                                    sequencia = await PesquisaClientesNoCadastro(telefone.Trim());
                                }

                                sequencia.numConta = Convert.ToInt32(reader["CONTA"].ToString());

                                if (sequencia.DeliveryAddress.FormattedAddress != null)
                                {
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
                                                           Convert.ToDecimal(reader["TAXAENTREGA"]) -
                                                           Convert.ToDecimal(reader["TAXAMOTOBOY"]) -
                                                           Convert.ToDecimal(reader["CORTESIA"]);
                                    string NumConta = sequencia.numConta.ToString();
                                    sequencia.IDPEDIDO = reader["iFoodPedidoID"].ToString();

                                    var idPedido = NumConta.PadLeft(4, '0') + "-" + DateTime.Now.ToString().Substring(0, 10).Replace("-", "/");

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

                                    var ValorDePagamentoOnline = Convert.ToDecimal(reader["PAGONLINE"]);

                                    if (opcSistema.RetornoAut)
                                    {
                                        if (ValorDePagamentoOnline > 0)
                                        {
                                            sequencia.PagamentoNaEntrega = false;
                                        }
                                        else
                                        {
                                            sequencia.PagamentoNaEntrega = true;
                                        }
                                    }
                                    else
                                    {
                                        sequencia.PagamentoNaEntrega = false;
                                    }

                                    sequencias.Add(sequencia);
                                }

                            }


                        }

                    }
                }
                return sequencias;
            }
        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
            MessageBox.Show(ex.ToString(), "Erro Ao listar Pedidos Abertos");
        }

        return sequencias;
    }


    public async Task<List<Sequencia>> ListarPedidosJaEnviados(string? CodEntregador = "99", string? empresaId = "DelMatchId") //método que serve para enviarmos um pedido
    {
        List<Sequencia> sequencias = new List<Sequencia>();
        try
        {
            using (ApplicationDbContext dbPostgres = await _Context.GetContextoAsync())
            {
                ParametrosDoSistema? opcSistema = dbPostgres.parametrosdosistema.FirstOrDefault();

                string? caminhoBancoAccess = opcSistema.CaminhodoBanco;

                string entregador = "99";
                string SqlSelectIntoCadastros = $"SELECT * FROM Sequencia WHERE TRIM(ENTREGADOR) = @ENTREGADOR AND {empresaId} IS NOT NULL";

                using (OleDbConnection connection = new OleDbConnection(caminhoBancoAccess))
                {
                    connection.Open();

                    using (OleDbCommand selectCommand = new OleDbCommand(SqlSelectIntoCadastros, connection))
                    {
                        selectCommand.Parameters.AddWithValue("@ENTREGADOR", CodEntregador);

                        // Executar a consulta SELECT
                        using (OleDbDataReader reader = selectCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string telefone = reader["TELEFONE"].ToString();
                                Sequencia sequencia = await PesquisaEnderecoDeEntrega(reader["CONTA"].ToString(), "ENVIARPEDIDO"); //PesquisaClientesNoCadastro(telefone.Trim());

                                if (sequencia.DeliveryAddress.FormattedAddress == null || sequencia.DeliveryAddress.FormattedAddress.Length <= 3)
                                {
                                    sequencia = await PesquisaClientesNoCadastro(telefone.Trim());
                                }

                                sequencia.numConta = Convert.ToInt32(reader["CONTA"].ToString());

                                if (sequencia.DeliveryAddress.FormattedAddress != null)
                                {
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
                                                           Convert.ToDecimal(reader["TAXAENTREGA"]) -
                                                           Convert.ToDecimal(reader["TAXAMOTOBOY"]) -
                                                           Convert.ToDecimal(reader["CORTESIA"]);
                                    string NumConta = sequencia.numConta.ToString();

                                    sequencia.IDPEDIDO = reader["iFoodPedidoID"].ToString();

                                    var idPedido = NumConta.PadLeft(4, '0') + "-" + DateTime.Now.ToString().Substring(0, 10).Replace("-", "/");

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

                                    sequencia.Machine_ID = reader["MachineId"].ToString();

                                    sequencias.Add(sequencia);
                                }

                            }


                        }

                    }
                }
                return sequencias;
            }
        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
            MessageBox.Show(ex.ToString(), "Erro Ao listar Pedidos Abertos");
        }

        return sequencias;
    }

    public static float TaxaDeGarcom(float valorDaConta, string? mesa)
    {
        float taxa = 0f;
        float ValorTaxa = 0;
        try
        {
            using (ApplicationDbContext dbPostgres = new ApplicationDbContext())
            {
                var mesaSelecionada = dbPostgres.mesas.FirstOrDefault(x => x.Codigo == mesa);

                if (mesaSelecionada is not null)
                {
                    if (!mesaSelecionada.Taxa)
                        return 0f;

                }

                ParametrosDoSistema? opcSistema = dbPostgres.parametrosdosistema.FirstOrDefault();
                string? caminhoBancoAccess = opcSistema.CaminhodoBanco.Replace("CONTAS", "SETUP");

                string SqlSelectIntoCadastros = $"SELECT * FROM Config";

                using (OleDbConnection connection = new OleDbConnection(caminhoBancoAccess))
                {
                    connection.Open();

                    using (OleDbCommand selectCommand = new OleDbCommand(SqlSelectIntoCadastros, connection))
                    {
                        using (OleDbDataReader reader = selectCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                taxa = Convert.ToSingle(reader["TAXASERVICO"].ToString());
                            }

                            if (taxa > 0)
                            {
                                taxa = taxa / 100;
                                ValorTaxa = valorDaConta * taxa;

                                return ValorTaxa;
                            }

                        }

                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Logs.CriaLogDeErro(ex.Message);
        }
        return ValorTaxa;
    }

    public async void UpdateMachineId(List<ClsApoioUpdateId> ClsDeApoioUpdate)
    {
        try
        {
            using (ApplicationDbContext dbPostgres = await _Context.GetContextoAsync())
            {
                ParametrosDoSistema? opcSistema = dbPostgres.parametrosdosistema.ToList().FirstOrDefault();

                string? caminhoBancoAccess = opcSistema.CaminhodoBanco;

                string updateQuery = "UPDATE Sequencia SET MachineId = @NovoValor WHERE CONTA = @CONDICAO;";

                using (OleDbConnection connection = new OleDbConnection(caminhoBancoAccess))
                {
                    // Abrindo a conexão
                    connection.Open();
                    foreach (var apoio in ClsDeApoioUpdate)
                    {
                        if (!apoio.EPedidoAgrupado)
                            using (OleDbCommand command = new OleDbCommand(updateQuery, connection))
                            {
                                string? MachineId = apoio.MachineId;
                                string? NumConta = apoio.NumConta.ToString();

                                // Definindo os parâmetros para a instrução SQL
                                command.Parameters.AddWithValue("@NovoValor", MachineId);
                                command.Parameters.AddWithValue("@CONDICAO", NumConta);

                                // Executando o comando UPDATE
                                command.ExecuteNonQuery();
                            }

                        if (apoio.EPedidoAgrupado)
                            foreach (var pedido in apoio.NumerosDeConta)
                            {
                                using (OleDbCommand command = new OleDbCommand(updateQuery, connection))
                                {
                                    string? MachineId = apoio.MachineId;
                                    string? NumConta = pedido.ToString();

                                    // Definindo os parâmetros para a instrução SQL
                                    command.Parameters.AddWithValue("@NovoValor", MachineId);
                                    command.Parameters.AddWithValue("@CONDICAO", NumConta);

                                    // Executando o comando UPDATE
                                    command.ExecuteNonQuery();
                                }
                            }
                    }
                }

            }
        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
            MessageBox.Show("Erro ao encontra Id. Por favor comunique o suporte da syslogica", "Ops", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

}

//Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\SAAB\BASE\CONTAS.mdb
