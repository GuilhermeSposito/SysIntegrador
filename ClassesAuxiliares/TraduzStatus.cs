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
                StatusTraduzido = "Solicitado Cancelamento";
                    break; 
            case "CANCELLATION_REQUEST_FAILED":
                StatusTraduzido = "Cancelamento Falhou";
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
            case "DELIVERY_DROP_CODE_REQUESTED":
                StatusTraduzido = "Código foi informado";
                break;
            case "GOING_TO_ORIGIN":
                StatusTraduzido = "Entregador a caminho";
                break;
            case "ARRIVED_AT_DESTINATION":
                StatusTraduzido = "Entregador Chegou no destino";
                break;
            case "DELIVERY_RETURNING_TO_ORIGIN":
                StatusTraduzido = "Entregador Retornando";
                break;
            case "DELIVERY_CANCELLATION_REQUESTED":
                StatusTraduzido = "Cancelamento De Entregador";
                break;
            case "DELIVERY_DROP_CODE_VALIDATION_SUCCESS":
                StatusTraduzido = "Código Informado com sucesso";
                break;
            default:
                StatusTraduzido = status; 
                break;
        }

        return StatusTraduzido; 

    }
}