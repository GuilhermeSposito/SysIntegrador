using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoOnPedido;

public class TokenOnPedido
{
    [JsonIgnore] public static string? TokenDaSessao { get; set; }
    [JsonProperty("status")] public bool Status { get; set; } 
    [JsonProperty("accessOAuthToken")] public string? AccessOAuthToken { get; set; } 
    [JsonProperty("type")] public string? Type { get; set; } 
    [JsonProperty("expiresIn")] public string? ExpiresIn { get; set; }

    public TokenOnPedido(){}
}

