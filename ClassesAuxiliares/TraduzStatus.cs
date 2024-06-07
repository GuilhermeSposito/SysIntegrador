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
                StatusTraduzido = "Pendente";
                break;
            case "READY_TO_PICKUP":
                StatusTraduzido = "Pronto Para Retirar";
                break;
            case "DISPATCHED":
                StatusTraduzido = "Despachado";
                break;
            case "CANCELLATION_REQUESTED":
                StatusTraduzido = "Cancelado";
                    break;
            case "REQUEST_DRIVER_SUCCESS":
                StatusTraduzido = "Entregador Chamado";
                break;
            case "REQUEST_DRIVER":
                StatusTraduzido = "Chamando Entregador";
                break;
            case "DELIVERED":
                StatusTraduzido = "Concluido";
                break;
            case "HANDSHAKE_SETTLEMENT":
                StatusTraduzido = "Disputa Concluida";
                break;
            case "HANDSHAKE_DISPUTE":
                StatusTraduzido = "Disputa Aberta";
                break;
            default:
                StatusTraduzido = status; 
                break;
        }

        return StatusTraduzido; 

    }
}