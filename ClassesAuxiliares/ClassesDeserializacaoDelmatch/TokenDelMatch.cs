using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoDelmatch;

public class TokenDelMatch
{
    [JsonIgnore] public static string? TokenDaSessao { get; set; }
    [JsonProperty("success")] public bool Success { get; set; } 
    [JsonProperty("message")] public string? Message { get; set; } 
    [JsonProperty("token")] public string? Token { get; set; } 
}
