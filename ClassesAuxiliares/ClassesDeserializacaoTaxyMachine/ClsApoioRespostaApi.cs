using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoTaxyMachine;

public class ClsApoioRespostaApi
{
    [JsonIgnore] public int NumConta {  get; set; }
    [JsonProperty("success")] public bool Success { get; set; }
    [JsonProperty("response")] public Response Response { get; set; } = new Response();
    [JsonProperty("errors")] public List<Errors> Errors { get; set; } = new List<Errors>();

}

public class Response
{
    [JsonProperty("id_mch")] public string? Machine_Id { get; set; }
}

public class Errors
{
    [JsonProperty("code")] public int Code { get; set; }
    [JsonProperty("message")] public string? Message { get; set; }

}
