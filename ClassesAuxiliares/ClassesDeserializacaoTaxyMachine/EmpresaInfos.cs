using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoTaxyMachine;

public class EmpresaInfos
{
    [JsonProperty("response")] public Infos Infos { get; set; } = new Infos(); 
}

public class Infos
{
    [JsonProperty("id")] public string? Id { get; set; } 
    [JsonProperty("nome")] public string? Nome { get; set; } 
    [JsonProperty("numero_contrato")] public string? NumeroContrato { get; set; } 
    [JsonProperty("endereco")] public string? Endereco { get; set; } 
    [JsonProperty("bairro")] public string? Bairro { get; set; } 
    [JsonProperty("cidade")] public string? Cidade { get; set; } 
    [JsonProperty("uf")] public string? Estado { get; set; } 
    [JsonProperty("lng")] public string? Longitude { get; set; } 
    [JsonProperty("lat")] public string? Latitude { get; set; } 

}
