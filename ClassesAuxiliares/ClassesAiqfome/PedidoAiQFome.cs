using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysIntegradorApp.ClassesAuxiliares.ClassesAiqfome;

public class PedidoAiQFome
{
    [JsonProperty("data")] public DataPedido Data { get; set; } = new();
}

public class DataPedido
{
    [JsonProperty("id")] public int Id { get; set; }
    [JsonProperty("status")] public string? Status { get; set; } = string.Empty;
    [JsonProperty("delivery_type")] public string? DeliveryType { get; set; } = string.Empty;
    [JsonProperty("document_receipt")] public string? DocumentReceipt { get; set; }
    [JsonProperty("pickup_at")] public string? PickupAt { get; set; }
    [JsonProperty("scheduled_for")] public string? ScheduledFor { get; set; }
    [JsonProperty("is_aiqentrega")] public bool IsAiqentrega { get; set; }
    [JsonProperty("store")] public List<Store> store { get; set; } = new();
    [JsonProperty("user")] public User User { get; set; } = new();
    [JsonProperty("timeline")] public Timeline Timeline { get; set; } = new();
    [JsonProperty("payment_method")] public PaymentMethod PaymentMethod { get; set; } = new();
    [JsonProperty("total")] public float Total { get; set; }
    [JsonProperty("subtotal")] public float Subtotal { get; set; }
    [JsonProperty("change")] public float Change { get; set; }
    [JsonProperty("delivery_tax")] public float DeliveryTax { get; set; }
    [JsonProperty("address")] public Address Endereco { get; set; } = new();
    [JsonProperty("coupon_value")] public float CouponValue { get; set; }
    [JsonProperty("original_total")] public float OriginalTotal { get; set; }
    [JsonProperty("original_subtotal")] public float OriginalSubtotal { get; set; }
    [JsonProperty("taxable_total")] public float TaxableTotal { get; set; }
    [JsonProperty("service_fee")] public ServiceFee ServiceFee { get; set; } = new();
    [JsonProperty("items")] public List<ItemAiQFome> Items { get; set; } = new();
    [JsonProperty("cancelled_items")] public List<ItemAiQFome> CancelledItems { get; set; } = new();
    [JsonProperty("replaced_items")] public List<ItemAiQFome> ReplacedItems { get; set; } = new();
    [JsonProperty("call")] public Call Call { get; set; } = new();
}




public class Address
{
    [JsonProperty("street_name")] public string? StreetName { get; set; } = string.Empty;
    [JsonProperty("number")] public string? Number { get; set; } = string.Empty;
    [JsonProperty("complement")] public string? Complement { get; set; } = string.Empty;
    [JsonProperty("reference")] public string? Reference { get; set; } = string.Empty;
    [JsonProperty("zipcode")] public string? Zipcode { get; set; } = string.Empty;
    [JsonProperty("phone")] public string? Phone { get; set; } = string.Empty;
    [JsonProperty("mobile_phone")] public string? MobilePhone { get; set; } = string.Empty;
    [JsonProperty("neighborhood_name")] public string? NeighborhoodName { get; set; } = string.Empty;
}

public class ItemAiQFome
{
    [JsonProperty("id")] public int Id { get; set; }
    [JsonProperty("name")] public string? Name { get; set; }
    [JsonProperty("sku")] public string? Sku { get; set; }
    [JsonProperty("category")] public string? Category { get; set; }
    [JsonProperty("size")] public string? Size { get; set; }
    [JsonProperty("value")] public float Value { get; set; }
    [JsonProperty("quantity")] public int Quantity { get; set; }
    [JsonProperty("discount_tax")] public float? DiscountTax { get; set; }
    [JsonProperty("description")] public string? Description { get; set; }
    [JsonProperty("observations")] public string? Observations { get; set; }
    [JsonProperty("status")] public int Status { get; set; }
    [JsonProperty("reason")] public string? Reason { get; set; }
    [JsonProperty("parent_id")] public int? ParentId { get; set; }
    [JsonProperty("package_value")] public float PackageValue { get; set; }
    [JsonProperty("additional_items")] public List<Additional> Additional { get; set; } = new();
    [JsonProperty("mandatory_items")] public List<Mandatory> Mandatory { get; set; } = new();
    [JsonProperty("sub")] public List<ItemAiQFome> Sub { get; set; } = new();
}

public class Additional
{
    [JsonProperty("sku")] public string? Sku { get; set; }
    [JsonProperty("name")] public string? Name { get; set; }
    [JsonProperty("value")] public float Value { get; set; }
}

public class Mandatory
{
    [JsonProperty("sku")] public string? Sku { get; set; }
    [JsonProperty("name")] public string? Name { get; set; }
    [JsonProperty("group")] public string? Group { get; set; }
    [JsonProperty("value")] public float Value { get; set; }
}



public class ServiceFee
{
    [JsonProperty("ride")] public string? Ride { get; set; }
    [JsonProperty("store")] public float Store { get; set; }
    [JsonProperty("aiq")] public float Aiq { get; set; }
    [JsonProperty("total")] public float Total { get; set; }
}



