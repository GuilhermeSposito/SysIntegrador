using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoDelmatch;

public class ClsParaPedirToken
{
    [JsonProperty("client_id")] public string? ClientId { get; set; }
    [JsonProperty("client_secret")] public string? ClientSecret { get; set; }
    [JsonProperty("grant_type")] public string? GrantType { get; set; }
    [JsonProperty("username")] public string? Username { get; set; }
    [JsonProperty("password")] public string? Password { get; set; }
}
