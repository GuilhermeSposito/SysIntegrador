using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoDelmatch;

public class ClsDeserializacaoPedidoFalho
{

    [JsonProperty("success")] public bool Success { get; set; }
    [JsonProperty("response")] public string Response { get; set; }

    public ClsDeserializacaoPedidoFalho()
    {

    }
}

public class Response
{
    [JsonProperty("code")] public int Code { get; set; }
    [JsonProperty("message")] public string? Message { get; set; }

}