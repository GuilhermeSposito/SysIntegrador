using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoOnPedido;

public class PollingOnPedido
{
    [JsonProperty("status")] public bool Status { get; set; }
    [JsonProperty("return")] public List<ReturnPooling> Return { get; set; } = new List<ReturnPooling>();
}

public class ReturnPooling
{
    [JsonProperty("eventId")] public string? EventId { get; set; }
    [JsonProperty("eventType")] public string? EventType { get; set; }
    [JsonProperty("orderId")] public int orderId { get; set; }
    [JsonProperty("orderURL")] public string? OrderURL { get; set; }
    [JsonProperty("createdAt")] public string? CreatedAt { get; set; }
    [JsonProperty("sourceAppId")] public string? SourceAppId { get; set; }
    public ReturnPooling(){ }
}