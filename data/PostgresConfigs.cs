using SysIntegradorApp.ClassesAuxiliares;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysIntegradorApp.data;

public class PostgresConfigs
{
    public static void LimparPedidos(List<ParametrosDoPedido>? pedidos = null)
    {
        try
        {
            using ApplicationDbContext db = new ApplicationDbContext();

            if (pedidos != null)
            {
                db.parametrosdopedido.RemoveRange(pedidos);
                db.SaveChanges();
            }
            else
            {
                List<ParametrosDoPedido> pedidosRemovidoManualmente = db.parametrosdopedido.ToList();
                db.parametrosdopedido.RemoveRange(pedidosRemovidoManualmente);
                db.SaveChanges();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Ops");
        }
    }

    public static void LimpaPedidosACada8horas()
    {
        try
        {
            using ApplicationDbContext db = new ApplicationDbContext();

            TimeSpan intervalo = TimeSpan.Parse("08:00:00"); // 8 horas

            var pedidosQuery = db.parametrosdopedido.AsQueryable();

            List<ParametrosDoPedido> pedidos = pedidosQuery
                .AsEnumerable() // Projetar para o lado do cliente
                .Where(p => DateTime.Now - DateTime.Parse(p.CriadoEm) > intervalo)
                .ToList();


            if (pedidos.Count() > 0)
            {
                LimparPedidos(pedidos);
                FormMenuInicial.panelDetalhePedido.Controls.Clear();
                FormMenuInicial.panelDetalhePedido.Controls.Add(FormMenuInicial.labelDeAvisoPedidoDetalhe);
                FormMenuInicial.panelPedidos.Controls.Clear();
                FormMenuInicial.panelPedidos.Invoke(new Action(async () => FormMenuInicial.SetarPanelPedidos()));
            }


        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Ops");
        }
    }


}
