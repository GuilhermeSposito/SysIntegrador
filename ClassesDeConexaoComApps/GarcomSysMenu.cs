using Microsoft.EntityFrameworkCore;
using SysIntegradorApp.ClassesAuxiliares;
using SysIntegradorApp.ClassesAuxiliares.ClassesGarcomSysMenu;
using SysIntegradorApp.ClassesAuxiliares.logs;
using SysIntegradorApp.data;
using SysIntegradorApp.data.InterfaceDeContexto;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysIntegradorApp.ClassesDeConexaoComApps;

public class GarcomSysMenu
{
    private readonly IMeuContexto _Context;

    public GarcomSysMenu(MeuContexto contexto)
    {
        _Context = contexto;
    }

    public async Task AtualizarListaDeGarcom()
    {
        try
        {
            using (ApplicationDbContext dbPostgres = await _Context.GetContextoAsync())
            {
                bool ExisteGarconNoPostgres = dbPostgres.garcons.Any();

                if (ExisteGarconNoPostgres)
                {
                    dbPostgres.garcons.RemoveRange(dbPostgres.garcons);
                    await dbPostgres.SaveChangesAsync();
                }

                ParametrosDoSistema? opcSistema = dbPostgres.parametrosdosistema.FirstOrDefault();
                string? caminhoBancoAccess = opcSistema.CaminhodoBanco.Replace("CONTAS", "CADASTROS");

                string SqlSelectIntoCadastros = $"SELECT * FROM Garcon";

                using (OleDbConnection connection = new OleDbConnection(caminhoBancoAccess))
                {
                    connection.Open();

                    using (OleDbCommand selectCommand = new OleDbCommand(SqlSelectIntoCadastros, connection))
                    {
                        using (OleDbDataReader reader = selectCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Garcom garcom = new Garcom();

                                garcom.Codigo = reader["CODIGO"].ToString();
                                garcom.Nome = reader["NOME"].ToString();
                                garcom.Senha = reader["SENHA"].ToString();
                                garcom.Valor = Convert.ToSingle(reader["VALOR"].ToString());

                                dbPostgres.garcons.Add(garcom);
                                await dbPostgres.SaveChangesAsync();
                            }

                        }

                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            await Logs.CriaLogDeErro(ex.Message);
        }
    }

    public async Task AtualizarListaDeIncrementos()
    {
        try
        {
            using (ApplicationDbContext dbPostgres = await _Context.GetContextoAsync())
            {
                bool ExisteIncrementos = dbPostgres.incrementos.Any();

                if (ExisteIncrementos)
                {
                    dbPostgres.incrementos.RemoveRange(dbPostgres.incrementos);
                    await dbPostgres.SaveChangesAsync();
                }

                ParametrosDoSistema? opcSistema = dbPostgres.parametrosdosistema.FirstOrDefault();
                string? caminhoBancoAccess = opcSistema.CaminhodoBanco.Replace("CONTAS", "CADASTROS");

                string SqlSelectIntoCadastros = $"SELECT * FROM Incrementos";

                using (OleDbConnection connection = new OleDbConnection(caminhoBancoAccess))
                {
                    connection.Open();

                    using (OleDbCommand selectCommand = new OleDbCommand(SqlSelectIntoCadastros, connection))
                    {
                        using (OleDbDataReader reader = selectCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Incremento incremento = new Incremento();

                                incremento.Codigo = reader["CODIGO"].ToString();
                                incremento.Descricao = reader["DESCRICAO"].ToString();
                                incremento.Valor = Convert.ToDouble(reader["VALOR"].ToString());
                                incremento.Tipo = reader["TIPO"].ToString();
                                incremento.VendaInternet = true;

                                dbPostgres.incrementos.Add(incremento);
                                await dbPostgres.SaveChangesAsync();
                            }

                        }

                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            await Logs.CriaLogDeErro(ex.Message);
        }
    }

    public async Task AtualizarListaDeIncrementosCardapio()
    {
        try
        {
            using (ApplicationDbContext dbPostgres = await _Context.GetContextoAsync())
            {
                bool ExisteIncrementosCardapio = dbPostgres.incrementocardapio.Any();

                if (ExisteIncrementosCardapio)
                {
                    dbPostgres.incrementocardapio.RemoveRange(dbPostgres.incrementocardapio);
                    await dbPostgres.SaveChangesAsync();
                }

                ParametrosDoSistema? opcSistema = dbPostgres.parametrosdosistema.FirstOrDefault();
                string? caminhoBancoAccess = opcSistema.CaminhodoBanco.Replace("CONTAS", "CADASTROS");

                string SqlSelectIntoCadastros = $"SELECT * FROM IncrementoCardapio";

                using (OleDbConnection connection = new OleDbConnection(caminhoBancoAccess))
                {
                    connection.Open();

                    using (OleDbCommand selectCommand = new OleDbCommand(SqlSelectIntoCadastros, connection))
                    {
                        using (OleDbDataReader reader = selectCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                IncrementoCardapio incrementoCard = new IncrementoCardapio();

                                incrementoCard.Incremento = reader["INCREMENTO"].ToString();
                                incrementoCard.CodCardapio = reader["CODCARDA"].ToString();
                               

                                dbPostgres.incrementocardapio.Add(incrementoCard);
                                await dbPostgres.SaveChangesAsync();
                            }

                        }

                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            await Logs.CriaLogDeErro(ex.Message);
        }
    }

    public async Task AtualizarProdutos()
    {
        try
        {
            using (ApplicationDbContext dbPostgres = await _Context.GetContextoAsync())
            {
                bool ExisteProdutosNoPostgres = dbPostgres.cardapio.Any();

                if (ExisteProdutosNoPostgres)
                {
                    dbPostgres.cardapio.RemoveRange(dbPostgres.cardapio);
                    await dbPostgres.SaveChangesAsync();
                }

                ParametrosDoSistema? opcSistema = dbPostgres.parametrosdosistema.FirstOrDefault();
                string? caminhoBancoAccess = opcSistema.CaminhodoBanco.Replace("CONTAS", "CADASTROS");

                string SqlSelectIntoCadastros = $"SELECT * FROM Cardapio";

                using (OleDbConnection connection = new OleDbConnection(caminhoBancoAccess))
                {
                    connection.Open();

                    using (OleDbCommand selectCommand = new OleDbCommand(SqlSelectIntoCadastros, connection))
                    {
                        using (OleDbDataReader reader = selectCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Produto produto = new Produto();

                                produto.Codigo = reader["CODIGO"].ToString();
                                produto.Descricao = reader["DESCRICAO"].ToString();
                                produto.Grupo = reader["GRUPO"].ToString();
                                produto.Fracionado = reader["FRACIONADO"].ToString();
                                produto.TamanhoUnico = reader["TAMUNICO"].ToString();
                                produto.Preco1 = Convert.ToSingle(reader["PVENDA1"].ToString());
                                produto.Preco2 = Convert.ToSingle(reader["PVENDA2"].ToString());
                                produto.Preco3 = Convert.ToSingle(reader["PVENDA3"].ToString());
                                produto.OcultaTablet = Convert.ToBoolean(reader["OCULTATABLET"].ToString());

                                dbPostgres.cardapio.Add(produto);
                                await dbPostgres.SaveChangesAsync();

                            }

                        }

                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            await Logs.CriaLogDeErro(ex.Message);
        }
    }

    public async Task AtualizarMesas()
    {
        try
        {
            using (ApplicationDbContext dbPostgres = await _Context.GetContextoAsync())
            {
                bool ExisteProdutosNoPostgres = dbPostgres.mesas.Any();

                if (ExisteProdutosNoPostgres)
                {
                    dbPostgres.mesas.RemoveRange(dbPostgres.mesas);
                    await dbPostgres.SaveChangesAsync();
                }

                ParametrosDoSistema? opcSistema = dbPostgres.parametrosdosistema.FirstOrDefault();
                string? caminhoBancoAccess = opcSistema.CaminhodoBanco.Replace("CONTAS", "CADASTROS");

                string SqlSelectIntoCadastros = $"SELECT * FROM Mesas WHERE TIPO = 'M'";

                using (OleDbConnection connection = new OleDbConnection(caminhoBancoAccess))
                {
                    connection.Open();

                    using (OleDbCommand selectCommand = new OleDbCommand(SqlSelectIntoCadastros, connection))
                    {
                        using (OleDbDataReader reader = selectCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Mesa mesa = new Mesa();

                                mesa.Codigo = reader["CODIGO"].ToString();
                                mesa.Praca = reader["PRACA"].ToString();
                                mesa.Tipo = reader["TIPO"].ToString();
                                mesa.status = reader["STATUS"].ToString();
                                mesa.Cartao = reader["CARTAO"].ToString();
                                mesa.Bloqueado = Convert.ToBoolean(reader["BLOQUEADO"].ToString());
                                mesa.Consumacao = Convert.ToSingle(reader["CONSUMACAO"].ToString());
                                mesa.Vip = Convert.ToBoolean(reader["VIP"].ToString());

                                dbPostgres.mesas.Add(mesa);
                                await dbPostgres.SaveChangesAsync();

                            }

                        }

                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            await Logs.CriaLogDeErro(ex.Message);
        }
    }

    public async Task AtualizarContas()
    {
        try
        {

            using (ApplicationDbContext dbPostgres = await _Context.GetContextoAsync())
            {
                bool ExisteDadosNoContaDoPostgres = dbPostgres.contas.Any();

                if (ExisteDadosNoContaDoPostgres)
                {
                    dbPostgres.contas.RemoveRange(dbPostgres.contas);
                    await dbPostgres.SaveChangesAsync();
                }

                ParametrosDoSistema? opcSistema = dbPostgres.parametrosdosistema.FirstOrDefault();
                string? caminhoBancoAccess = opcSistema!.CaminhodoBanco;

                string SqlSelectIntoContas = $"SELECT * FROM Contas";

                using (OleDbConnection connection = new OleDbConnection(caminhoBancoAccess))
                {
                    connection.Open();

                    using (OleDbCommand selectCommand = new OleDbCommand(SqlSelectIntoContas, connection))
                    {
                        using (OleDbDataReader reader = selectCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Contas Conta = new Contas();

                                Contas conta = new Contas();

                                conta.Conta = reader["CONTA"].ToString();
                                conta.Mesa = reader["MESA"].ToString();
                                conta.Qtdade = Convert.ToInt32(reader["QTDADE"].ToString());
                                conta.CodCarda1 = reader["CODCARDA1"].ToString();
                                conta.CodCarda2 = reader["CODCARDA2"].ToString();
                                conta.CodCarda3 = reader["CODCARDA3"].ToString();
                                conta.Tamanho = reader["TAMANHO"].ToString();
                                conta.Descarda = reader["DESCARDA"].ToString();
                                conta.ValorUnit = reader["VALORUNIT"].ToString();
                                conta.ValorTotal = reader["VALORTOTAL"].ToString();
                                conta.DataInicio = reader["DATAINICIO"].ToString();
                                conta.HoraInicio = reader["HORAINICIO"].ToString();
                                conta.Obs1 = reader["OBS1"].ToString();
                                conta.Obs2 = reader["OBS2"].ToString();
                                conta.Obs3 = reader["OBS3"].ToString();
                                conta.Obs4 = reader["OBS4"].ToString();
                                conta.Obs5 = reader["OBS5"].ToString();
                                conta.Obs6 = reader["OBS6"].ToString();
                                conta.Obs7 = reader["OBS7"].ToString();
                                conta.Obs8 = reader["OBS8"].ToString();
                                conta.Obs9 = reader["OBS9"].ToString();
                                conta.Obs10 = reader["OBS10"].ToString();
                                conta.Obs11 = reader["OBS11"].ToString();
                                conta.Obs12 = reader["OBS12"].ToString();
                                conta.Obs13 = reader["OBS13"].ToString();
                                conta.Obs14 = reader["OBS14"].ToString();
                                conta.Obs15 = reader["OBS15"].ToString();
                                conta.Cliente = reader["CLIENTE"].ToString();
                                conta.Status = reader["STATUS"].ToString();
                                conta.Telefone = reader["TELEFONE"].ToString();
                                conta.ImpComanda = reader["IMPCOMANDA"].ToString();
                                conta.ImpComanda2 = reader["IMPCOMANDA2"].ToString();
                                conta.QtdComanda = Convert.ToSingle(reader["QTDCOMANDA"].ToString());
                                conta.Usuario = reader["USUARIO"].ToString();

                                dbPostgres.contas.Add(conta);
                                await dbPostgres.SaveChangesAsync();

                            }

                        }

                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            await Logs.CriaLogDeErro(ex.Message);
        }
    }

    public async Task AtualizaGrupos()
    {
        try
        {
            using (ApplicationDbContext dbPostgres = await _Context.GetContextoAsync())
            {
                bool ExisteProdutosNoPostgres = dbPostgres.grupos.Any();

                if (ExisteProdutosNoPostgres)
                {
                    dbPostgres.grupos.RemoveRange(dbPostgres.grupos);
                    await dbPostgres.SaveChangesAsync();
                }

                ParametrosDoSistema? opcSistema = dbPostgres.parametrosdosistema.FirstOrDefault();
                string? caminhoBancoAccess = opcSistema.CaminhodoBanco.Replace("CONTAS", "CADASTROS");

                string SqlSelectIntoCadastros = $"SELECT * FROM GruCard";

                using (OleDbConnection connection = new OleDbConnection(caminhoBancoAccess))
                {
                    connection.Open();

                    using (OleDbCommand selectCommand = new OleDbCommand(SqlSelectIntoCadastros, connection))
                    {
                        using (OleDbDataReader reader = selectCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Grupo grupo = new Grupo();

                                grupo.Codigo = reader["CODIGO"].ToString();
                                grupo.Descricao = reader["DESCRICAO"].ToString();
                                grupo.Familia = reader["FAMILIA"].ToString();
                                grupo.Oculta = Convert.ToBoolean(reader["OCULTATABLET"].ToString());
                                grupo.TOTGRUPO = Convert.ToDouble(reader["TOTGRUPO"].ToString());


                                dbPostgres.grupos.Add(grupo);
                                await dbPostgres.SaveChangesAsync();

                            }

                        }

                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            await Logs.CriaLogDeErro(ex.Message);
        }
    }

    public async Task AtualizarBancoDeDadosParaOGarcon()
    {
        await AtualizarListaDeGarcom();
        await AtualizaGrupos();
        await AtualizarMesas();
        await AtualizarProdutos();
        await AtualizarContas();
        await AtualizarListaDeIncrementos();
        await AtualizarListaDeIncrementosCardapio();
    }

}
