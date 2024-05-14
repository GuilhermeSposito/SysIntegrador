using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysIntegradorApp.ClassesAuxiliares;

public class ClsDeTraducaoDelMatchStatus
{
    public static string TraduzStatus(string? status)
    {
        string? statusTraduzido = "";

        switch (status)
        {
            case "Awaiting acceptance":
                statusTraduzido = "Esperando Aceitar";
                break;
            case "Não Enviado":
                statusTraduzido = "Não Enviado";
                break;
            case "Accepted":
                statusTraduzido = "Aceita";
                break;
            case "On hold":
                statusTraduzido = "Em espera";
                break;
            case "In progress":
                statusTraduzido = "Em andamento";
                break;
            case "Finished":
                statusTraduzido = "Finalizada";
                break;
            case "Not answered":
                statusTraduzido = "Não atendida";
                break;  
            case "Canceled":
                statusTraduzido = "Cancelada";
                break; 
            case "Pending":
                statusTraduzido = "Pendente";
                break;
            default:
                statusTraduzido = status;
                break;
        }

        return statusTraduzido;
    }
}
