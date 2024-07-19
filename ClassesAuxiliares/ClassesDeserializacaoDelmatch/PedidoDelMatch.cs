using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoOnPedido;

namespace SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoDelmatch;

public class PedidoDelMatch
{
    [JsonIgnore] public int NumConta { get; set; }  
    [JsonProperty("id")] public int Id { get; set; } 
    [JsonProperty("reference")] public int Reference { get; set; } 
    [JsonProperty("createdAt")] public string? CreatedAt { get; set; } 
    [JsonProperty("type")] public string? Type { get; set; } 
    [JsonProperty("merchant")] public Merchant Merchant { get; set; } = new Merchant();
    [JsonProperty("address")] public Adress Address { get; set; } = new Adress();
    [JsonProperty("payments")] public List<Payments> Payments { get; set; } = new List<Payments>();
    [JsonProperty("customer")] public Customer Customer { get; set; } = new Customer();
    [JsonProperty("items")] public List<items> Items { get; set; } = new List<items>();
    [JsonProperty("subTotal")] public float SubTotal { get; set; }
    [JsonProperty("discount")] public float Discount { get; set; }
    [JsonProperty("additionalFee")] public float AdditionalFee { get; set; }
    [JsonProperty("totalPrice")] public float TotalPrice { get; set; }
    [JsonProperty("deliveryFee")] public float deliveryFee { get; set; } 
    [JsonProperty("deliveryAddress")] public deliveryAddress deliveryAddress { get; set; } = new deliveryAddress();
    [JsonProperty("deliveryDateTime")] public string? DeliveryDateTime { get; set; } 
    [JsonProperty("preparationTimeInSeconds")] public int PreparationTimeInSeconds { get; set; } 
    [JsonProperty("mesa_comanda")] public string? Mesa_comanda { get; set; } 
    [JsonProperty("scheduleDateTime")] public string? ScheduleDateTime { get; set; } 
    [JsonProperty("orderType")] public string? orderType { get; set; } 
    [JsonProperty("takeout")] public Takeout Takeout { get; set; } = new Takeout();
    [JsonProperty("indoor")] public SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoDelmatch.Indoor Indoor { get; set; } = new SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoDelmatch.Indoor();
}

public class Merchant
{
    [JsonProperty("merchant")] public string? merchant { get; set; } 
    [JsonProperty("name")] public string? Name { get; set; }
    [JsonProperty("phones")] public List<string>? Phones { get; set; } = new List<string>();
    [JsonProperty("address")] public Adress Address { get; set; } = new Adress();

    public Merchant() { }
}

public class Adress
{
    [JsonProperty("formattedAddress")] public string? FormattedAddress { get; set; }
    [JsonProperty("country")] public string? Country { get; set; }
    [JsonProperty("satate")] public string? State { get; set; }
    [JsonProperty("city")] public string? City { get; set; }
    [JsonProperty("neighboardhood")] public string? Neighboardhood { get; set; }
    [JsonProperty("streetName")] public string? StreetName { get; set; }
    [JsonProperty("streetNumber")] public string? StreetNumber { get; set; }
    [JsonProperty("qrcode")] public string? Qrcode { get; set; }
    [JsonProperty("link")] public string? Link { get; set; }

    public Adress() { }
}

public class Payments
{
    [JsonProperty("name")] public string? Name { get; set; }
    [JsonProperty("code")] public string? Code { get; set; }
    [JsonProperty("prepaid")] public bool Prepaid { get; set; }
    [JsonProperty("value")] public float Value { get; set; }
    [JsonProperty("issuer")] public string? Issuer { get; set; }
    [JsonProperty("cashChange")] public float CashChange { get; set; }

    public Payments() { }
}

public class Customer
{
    [JsonProperty("id")] public string? Id { get; set; }
    [JsonProperty("name")] public string? Name { get; set; }
    [JsonProperty("taxPayerIdentificationNumber")] public string? CPF { get; set; }
    [JsonProperty("phone")] public string? Phone { get; set; }
    [JsonProperty("email")] public string? Email { get; set; }
    [JsonProperty("total_orders")] public int Total_Orders { get; set; }

    public Customer() { }
}

public class items
{
    [JsonProperty("item_id")] public int Item_Id { get; set; }
    [JsonProperty("id")] public int Id { get; set; }
    [JsonProperty("name")] public string? Name { get; set; }
    [JsonProperty("quantity")] public int Quantity { get; set; }
    [JsonProperty("price")] public float Price { get; set; }
    [JsonProperty("subItemsPrice")] public float SubItemsPrice { get; set; }
    [JsonProperty("totalPrice")] public float TotalPrice { get; set; }
    [JsonProperty("discount")] public double Discount { get; set; }
    [JsonProperty("addition")] public double Addition { get; set; }
    [JsonProperty("is_read")] public bool Is_Read { get; set; } = false;
    [JsonProperty("externalCode")] public string? ExternalCode { get; set; }
    [JsonProperty("observations")] public string? Observations { get; set; }
    [JsonProperty("subItems")] public List<SubItens> SubItems { get; set; } = new List<SubItens>(); 


    public items() { }
}

public class SubItens
{
    [JsonProperty("name")] public string? Name { get; set; }
    [JsonProperty("quantity")] public int Quantity { get; set; }
    [JsonProperty("totalPrice")] public string? TotalPrice { get; set; }
    [JsonProperty("price")] public float Price { get; set; }
    [JsonProperty("discount")] public double Discount { get; set; }
    [JsonProperty("addition")] public float Addition { get; set; }
    [JsonProperty("externalCode")] public string? ExternalCode { get; set; }
    [JsonProperty("observations")] public string? Observations { get; set; }
    [JsonProperty("group")] public string? Group { get; set; }
    [JsonProperty("group_id")] public string? Group_Id { get; set; }
    [JsonProperty("type")] public int Type { get; set; }

    public SubItens(){}
}

public class deliveryAddress()
{
    [JsonProperty("formattedAddress")] public string? FormattedAddress { get; set; }
    [JsonProperty("country")] public string? Country { get; set; }
    [JsonProperty("state")] public string? State { get; set; }
    [JsonProperty("city")] public string? City { get; set; }
    [JsonProperty("coordinates")] public List<Coordinates>? Coordinates { get; set; } = new List<Coordinates>();
    [JsonProperty("neighboardhood")] public string? Neighboardhood { get; set; }
    [JsonProperty("neighborhood")] public string? Neighborhood { get; set; }
    [JsonProperty("streetName")] public string? StreetName { get; set; }
    [JsonProperty("streetNumber")] public string? StreetNumber { get; set; }
    [JsonProperty("postalCode")] public string? PostalCode { get; set; }
    [JsonProperty("reference")] public string? Reference { get; set; }
    [JsonProperty("complement")] public string? Complement { get; set; }
    [JsonProperty("qrcode")] public string? Qrcode { get; set; }
    [JsonProperty("link")] public string? Link { get; set; }

}

public class Coordinates
{
    public float latitude { get; set; }
    public float longitude { get; set; }

    public Coordinates(){  }
}

public class Takeout
{
    [JsonProperty("deliveredBy")] public string? DeliveredBy { get; set; }
    [JsonProperty("mode")] public string? Mode { get; set; }
    
}

public class Indoor
{
    [JsonProperty("mode")] public string? Mode { get; set; } 
    [JsonProperty("table")] public string? table { get; set; }
   // [JsonProperty("commander")] public string? commander { get; set; }

}

public class ClsParaConfirmarItem
{
    [JsonProperty("itens")] public List<int> Itens { get; set; } = new List<int>();        

}