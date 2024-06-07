using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoOnPedido;

public class ClsParaConfirmarPedido
{
    [JsonProperty("reason")] public string? Reason { get; set; }
    [JsonProperty("createdAt")] public string? CreatedAt { get; set; }
    [JsonProperty("orderExternalCode")] public string? OrderExternalCode { get; set; }
    [JsonProperty("preparationTime")] public int PreparationTime { get; set; }

    public ClsParaConfirmarPedido() { }

    public ClsParaConfirmarPedido(string reason, string createAt, string orderExternalCode, int time)
    {
        Reason = reason;
        CreatedAt = createAt;
        OrderExternalCode = orderExternalCode;
        PreparationTime = time;
    }

}
