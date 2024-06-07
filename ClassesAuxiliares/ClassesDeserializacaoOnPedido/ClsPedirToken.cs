using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoOnPedido;


public class ClsPedirToken  
{
    [JsonProperty("merchantOAuthToken")] public string? MerchantOAuthToken { get; set; } 
    [JsonProperty("softwareOAuthToken")] public string? SoftwareOAuthToken { get; set; } 
    [JsonProperty("merchantUsername")] public string? MerchantUsername { get; set; } 
    [JsonProperty("merchantPassword")] public string? MerchantPassword { get; set; } 
    [JsonProperty("clearAnotherTokens")] public bool ClearAnotherTokens { get; set; }

    public ClsPedirToken(){}
}
