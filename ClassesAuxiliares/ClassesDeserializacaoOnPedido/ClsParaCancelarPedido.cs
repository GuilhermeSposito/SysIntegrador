using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoOnPedido;

public class ClsParaCancelarPedido
{
    [JsonProperty("reason")] public string? Reason {  get; set; }    
    [JsonProperty("code")] public string? Code {  get; set; }    
    [JsonProperty("mode")] public string? Mode {  get; set; }
    [JsonProperty("outOfStockItems")] public List<string>? OutOfStockItems { get; set; } = new List<string>();
    [JsonProperty("invalidItems")] public List<string>? InvalidItems { get; set; } = new List<string>();

    public ClsParaCancelarPedido()
    {
            
    }
}
