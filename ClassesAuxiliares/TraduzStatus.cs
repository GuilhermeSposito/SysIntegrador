using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysIntegradorApp.ClassesAuxiliares;

internal class TraduzStatus
{
    public static string TraduzStatusEnviado(string status)
    {
        string? StatusTraduzido = status;

        switch (status)
        {
            case "CANCELLED":
                StatusTraduzido = "Cancelado";
                break;
            case "CONCLUDED":
                StatusTraduzido = "Concluido";
                break;
            case "CONFIRMED":
                StatusTraduzido = "Confirmado";
                break;
            case "PLACED":
                StatusTraduzido = "Novo";
                break;
            case "READY_TO_PICKUP":
                StatusTraduzido = "Pronto Para Retirar";
                break;
            case "DISPATCHED":
                StatusTraduzido = "Despachado";
                break;
        }

        return StatusTraduzido; 

    }
}