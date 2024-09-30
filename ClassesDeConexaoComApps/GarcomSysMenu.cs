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
    }

}
