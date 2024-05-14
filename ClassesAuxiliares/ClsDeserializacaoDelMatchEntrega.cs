using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysIntegradorApp.ClassesAuxiliares;

public class ClsDeserializacaoDelMatchEntrega
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("name_api")]
    public string? NameApi { get; set; }

    [JsonProperty("created")]
    public string? Created { get; set; }

    [JsonProperty("modified")]
    public string? Modified { get; set; }

    [JsonProperty("id_order")]
    public string? IdOrder { get; set; }

    [JsonProperty("observation_order")]
    public string? ObservationOrder { get; set; }

    [JsonProperty("id_cliente")]
    public string? IdCliente { get; set; }

    [JsonProperty("name")]
    public string? Name { get; set; }

    [JsonProperty("email")]
    public string? Email { get; set; }

    [JsonProperty("phone")]
    public string? Phone { get; set; }

    [JsonProperty("document_number")]
    public string? DocumentNumber { get; set; }

    [JsonProperty("street")]
    public string? Street { get; set; }

    [JsonProperty("number_address")]
    public string? NumberAddress { get; set; }

    [JsonProperty("cep_address")]
    public string? CepAddress { get; set; }

    [JsonProperty("district_address")]
    public string? DistrictAddress { get; set; }

    [JsonProperty("city_address")]
    public string? CityAddress { get; set; }

    [JsonProperty("state_address")]
    public string? StateAddress { get; set; }

    [JsonProperty("complemet_address")]
    public string? ComplemetAddress { get; set; }

    [JsonProperty("reference_address")]
    public string? ReferenceAddress { get; set; }

    [JsonProperty("status")]
    public string? Status { get; set; } 

    [JsonProperty("message")]
    public string? Message { get; set; }

    [JsonProperty("return_motoboy")]
    public bool? ReturnMotoboy { get; set; }

    [JsonProperty("id_machine")]
    public string? IdMachine { get; set; }

    [JsonProperty("id_machine_delivery")]
    public string? IdMachineDelivery { get; set; }

    [JsonProperty("origin")]
    public int Origin { get; set; }
}