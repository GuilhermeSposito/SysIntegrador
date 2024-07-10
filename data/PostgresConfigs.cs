using Microsoft.EntityFrameworkCore;
using SysIntegradorApp.ClassesAuxiliares;
using SysIntegradorApp.ClassesAuxiliares.logs;
using SysIntegradorApp.ClassesDeConexaoComApps;
using SysIntegradorApp.Forms.ONPEDIDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysIntegradorApp.data;

public class PostgresConfigs
{
    public static async Task LimparPedidos(List<ParametrosDoPedido>? pedidos = null)
    {
        try
        {
            using ApplicationDbContext db = new ApplicationDbContext();

            if (pedidos != null)
            {
                db.parametrosdopedido.RemoveRange(pedidos);
                await db.SaveChangesAsync();
            }
            else
            {
                List<ParametrosDoPedido> pedidosRemovidoManualmente = db.parametrosdopedido.ToList();
                db.parametrosdopedido.RemoveRange(pedidosRemovidoManualmente);
                await db.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.ToString());
            MessageBox.Show(ex.Message, "Ops");
        }
    }

    public static async Task LimpaPedidosACada8horas()
    {
        try
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                TimeSpan intervalo = TimeSpan.Parse("08:00:00"); // 8 horas

                var pedidosQuery = db.parametrosdopedido.AsQueryable();

                List<ParametrosDoPedido> pedidos = pedidosQuery
                    .AsEnumerable() // Projetar para o lado do cliente
                    .Where(p => DateTime.Now - DateTime.Parse(p.CriadoEm) > intervalo)
                    .ToList();


                if (pedidos.Count() > 0)
                {
                    await LimparPedidos(pedidos);
                    FormMenuInicial.panelDetalhePedido.Controls.Clear();
                    FormMenuInicial.panelDetalhePedido.Controls.Add(FormMenuInicial.labelDeAvisoPedidoDetalhe);
                    FormMenuInicial.panelPedidos.Controls.Clear();
                    FormMenuInicial.panelPedidos.Invoke(new Action(async () => FormMenuInicial.SetarPanelPedidos()));
                }

            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Ops");
        }
    }

    public static async Task ConcluiPedidoOnPedido()
    {
        try
        {
            using ApplicationDbContext db = new ApplicationDbContext();

            List<ParametrosDoSistema> ConfigsList = await db.parametrosdosistema.ToListAsync();
            ParametrosDoSistema? Configs = ConfigsList.FirstOrDefault();


            if (Configs.DtUltimaVerif != null)
            {
                DateTime HoraAtual = DateTime.Now;
                DateTime DtUltimaVerif = DateTime.Parse(Configs.DtUltimaVerif);

                TimeSpan diferenca = HoraAtual - DtUltimaVerif;

                if (diferenca.TotalMinutes > Configs.TempoConclonPedido)
                {
                    Configs.DtUltimaVerif = HoraAtual.ToString();
                    db.SaveChanges();

                    int totalMinutos = db.parametrosdosistema.FirstOrDefault().TempoConclonPedido;

                    int horas = totalMinutos / 60;
                    int minutos = totalMinutos % 60;
                    int segundos = 0;

                    string tempoFormatado = $"{horas:D2}:{minutos:D2}:{segundos:D2}";

                    TimeSpan intervalo = TimeSpan.Parse(tempoFormatado);

                    var pedidosQuery = db.parametrosdopedido.AsQueryable();

                    List<ParametrosDoPedido> pedidos = pedidosQuery
                        .AsEnumerable() // Projetar para o lado do cliente
                        .Where(p => DateTime.Now - DateTime.Parse(p.CriadoEm) > intervalo && p.Situacao != "CANCELLED" && p.Situacao != "DELIVERED" && p.CriadoPor == "ONPEDIDO")
                        .ToList();


                    if (pedidos.Count() > 0)
                    {

                    }

                }
            }



        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Ops");
        }
    }


}
