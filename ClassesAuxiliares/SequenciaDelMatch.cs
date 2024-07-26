using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text.Json;

namespace SysIntegradorApp.ClassesAuxiliares;

public class Sequencia
{
    [JsonIgnore]public int numConta {  get; set; }
    [JsonIgnore] public bool Retorno { get; set; } = false;
    [JsonIgnore] public string? Machine_ID { get; set; }
    
    [JsonIgnore]public decimal ValorConta {  get; set; }
    [JsonIgnore] public string? DelMatchId { get; set; }
 
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("shortReference")]
    public string ShortReference { get; set; }

    [JsonProperty("createdAt")]
    public string CreatedAt { get; set; }

    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("time_max")]
    public string TimeMax { get; set; }

    [JsonProperty("merchant")]
    public merchant Merchant { get; set; } = new merchant();

    [JsonProperty("customer")]
    public customer Customer { get; set; } = new customer();

    [JsonProperty("deliveryAddress")]
    public deliveryAddress DeliveryAddress { get; set; } = new deliveryAddress();
}

public class merchant
{
    [JsonProperty("restaurantId")]
    public string RestaurantId { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("unit")]
    public string Unit { get; set; }
}

public class customer
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("phone")]
    public string Phone { get; set; }

    [JsonProperty("taxPayerIdentificationNumber")]
    public string TaxPayerIdentificationNumber { get; set; }
}

public class deliveryAddress
{
    [JsonProperty("formattedAddress")]
    public string FormattedAddress { get; set; }

    [JsonProperty("country")]
    public string Country { get; set; }

    [JsonProperty("state")]
    public string State { get; set; }

    [JsonProperty("city")]
    public string City { get; set; }

    [JsonProperty("coordinates")]
    public coordinates Coordinates { get; set; } = new coordinates();

    [JsonProperty("neighborhood")]
    public string Neighborhood { get; set; }

    [JsonProperty("streetName")]
    public string StreetName { get; set; }

    [JsonProperty("streetNumber")]
    public string StreetNumber { get; set; }

    [JsonProperty("postalCode")]
    public string PostalCode { get; set; }

    [JsonProperty("complement")]
    public string Complement { get; set; }
}

public class coordinates
{
    [JsonProperty("latitude")]
    public double Latitude { get; set; }

    [JsonProperty("longitude")]
    public double Longitude { get; set; }
}
