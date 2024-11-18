using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoOnPedido;

public class ClsErroApi
{
    [JsonPropertyName("status")]public bool Status { get; set; }
    [JsonPropertyName("error")] public Error? Error { get; set; }
}

public class Error
{
    [JsonPropertyName("code")] public string? Code { get; set; }
    [JsonPropertyName("name")] public string? Name { get; set; }
    [JsonPropertyName("date")] public string? Date { get; set; }
}
