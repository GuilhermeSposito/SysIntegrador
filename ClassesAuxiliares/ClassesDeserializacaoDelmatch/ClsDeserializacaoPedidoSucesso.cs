using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoDelmatch;

public class ClsDeserializacaoPedidoSucesso
{
    [JsonProperty("success")] public bool Success { get; set; } 
    [JsonProperty("response")] public string Response { get; set; }

    public ClsDeserializacaoPedidoSucesso()
    {
            
    }
}
