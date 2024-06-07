using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysIntegradorApp.ClassesAuxiliares;

public class ClsInfosDePagamentosParaImpressao
{
    public string? FormaPagamento { get; set; }
    public string? TipoPagamento { get; set; }
    public double valor { get; set; }

    public ClsInfosDePagamentosParaImpressao()
    {

    }
    public static ClsInfosDePagamentosParaImpressao DefineTipoDePagamento(List<Methods> metodos)
    {
        ClsInfosDePagamentosParaImpressao infos = new ClsInfosDePagamentosParaImpressao();
        foreach (Methods metodo in metodos)
        {
            switch (metodo.type)
            {
                case "ONLINE":
                    infos.TipoPagamento = "Pago Online";
                    break;
                case "OFFLINE":
                    infos.TipoPagamento = "VAI SER PAGO NA ENTREGA";
                    break;
            }


            switch (metodo.method)
            {
                case "CREDIT":
                    infos.FormaPagamento = "(Crédito)";
                    break;
                case "MEAL_VOUCHER":
                    infos.FormaPagamento = "(VOUCHER)";
                    break;
                case "DEBIT":
                    infos.FormaPagamento = "(Débito)";
                    break;
                case "PIX":
                    infos.FormaPagamento = "(PIX)";
                    break;
                case "CASH":
                    if (metodo.cash.changeFor > 0)
                    {
                        double totalTroco = metodo.cash.changeFor - metodo.value;
                        infos.FormaPagamento = $"(Dinheiro) Levar troco para {metodo.cash.changeFor.ToString("c")} Total Troco: {totalTroco.ToString("c")}";
                    }
                    else
                    {
                        infos.FormaPagamento = "(Dinheiro) Não precisa de troco";
                    }
                    break;
                case "BANK_PAY ":
                    infos.FormaPagamento = "(Bank Pay)";
                    break;
                case "FOOD_VOUCHER ":
                    infos.FormaPagamento = "(Vale Refeição)";
                    break;
                default:
                    infos.FormaPagamento = "(Online)";
                    break;

            }

            infos.valor = metodo.value;

        }

        return infos;
    }
}


public class ClsInfosDePagamentosParaImpressaoDelMatch
{
    public string? FormaPagamento { get; set; }
    public string? TipoPagamento { get; set; }
    public double valor { get; set; }

    public ClsInfosDePagamentosParaImpressaoDelMatch()
    {

    }
    public static ClsInfosDePagamentosParaImpressao DefineTipoDePagamento(List<SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoDelmatch.Payments> pagamentos)
    {
        ClsInfosDePagamentosParaImpressao infos = new ClsInfosDePagamentosParaImpressao();
        foreach (var metodo in pagamentos)
        {
            switch (metodo.Code)
            {
                case "online":
                    infos.TipoPagamento = metodo.Name;
                    break;
                case "DEB":
                    infos.TipoPagamento = metodo.Name;
                    break;
                default:
                    infos.TipoPagamento = metodo.Name;
                    break;
            }

            if (metodo.Code == "online")
            {
                infos.FormaPagamento = "Não é nescessario receber do cliente na entrega";
                continue;
            }

            if (metodo.CashChange > 0)
            {
                infos.FormaPagamento = $"Será pago na entrega em dinhero, Levar Troco: {metodo.CashChange.ToString("c")}";
                continue;
            }
            else
            {
                infos.FormaPagamento = $"Será pago na entrega";
                continue;
            }


        }

        return infos;
    }
}

public class ClsInfosDePagamentosParaImpressaoONPedido
{
    public string? FormaPagamento { get; set; }
    public string? TipoPagamento { get; set; }
    public double valor { get; set; }

    public ClsInfosDePagamentosParaImpressaoONPedido()
    {

    }
    public static ClsInfosDePagamentosParaImpressao DefineTipoDePagamento(SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoOnPedido.Payments pagamentos)
    {
        ClsInfosDePagamentosParaImpressao infos = new ClsInfosDePagamentosParaImpressao();

        bool PrePago = pagamentos.Prepaid > 0 && pagamentos.Pending == 0;

        foreach (var metodo in pagamentos.Methods)
        {
            if (PrePago)
            {
                infos.FormaPagamento = $"Pedido pago online, Não é nescessario Receber do cliente";
            }
            else
            {
                infos.FormaPagamento = $"Pedido Deverá ser cobrado na entrega";
            }

            if (PrePago)
            {
                infos.TipoPagamento = $"Pedido pago com ({metodo.Method}) valor {metodo.value}";
            }
            else
            {
                infos.TipoPagamento = $"Pedido Será pago com ({metodo.Method}) valor {metodo.value.ToString("c")}";
            }

            if (metodo.ChangeFor - metodo.value > 0)
            {
                infos.TipoPagamento += $" Levar troco  {(metodo.ChangeFor - metodo.value).ToString("c")}";
            }

        }

        return infos;
    }
}

