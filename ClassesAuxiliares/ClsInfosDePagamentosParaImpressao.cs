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
