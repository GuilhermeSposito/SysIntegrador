using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoAnotaAi;

public class PollingAnotaAI
{
    [JsonProperty("success")] public bool Success { get; set; }
    [JsonProperty("info")] public InfoAnotaAi InfoAnotaAi { get; set; } = new InfoAnotaAi();
}

public class InfoAnotaAi
{
    [JsonProperty("docs")] public List<InfosDoPooling> ListaDePedidos { get; set; } = new List<InfosDoPooling>();
}

public class InfosDoPooling
{
    [JsonProperty("_id")] public string? IdPedido { get; set; }
    [JsonProperty("check")] public int Check { get; set; }
    [JsonProperty("from")] public string? From { get; set; }
    [JsonProperty("salesChannel")] public string? SalesChannel { get; set; }
    [JsonProperty("updatedAt")] public string? UpdatedAt { get; set; }
}