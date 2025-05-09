using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysIntegradorApp.ClassesAuxiliares.ClassesAiqfome;

public class ClsPedidos
{
    [JsonProperty("data")] public List<Data> Data { get; set; } = new List<Data>();
    [JsonProperty("links")] public Links Links { get; set; } = new Links();
    [JsonProperty("meta")] public Meta Meta { get; set; } = new Meta();
}

public class Data
{
    [JsonProperty("id")] public int Id { get; set; }
    [JsonProperty("status")] public string Status { get; set; } = string.Empty;
    [JsonProperty("total")] public int Total { get; set; }
    [JsonProperty("change")] public int Change { get; set; }
    [JsonProperty("delivery_type")] public string DeliveryType { get; set; } = string.Empty;
   // [JsonProperty("coupon")] public object Coupon { get; set; } = new object();
    //[JsonProperty("pickup_at")] public object PickupAt { get; set; } = new object();
    //[JsonProperty("scheduled_for")] public object ScheduledFor { get; set; } = new object();
    [JsonProperty("is_aiqentrega")] public bool IsAiqentrega { get; set; }
    [JsonProperty("store")] public Store Store { get; set; } = new Store();
    [JsonProperty("timeline")] public Timeline Timeline { get; set; } = new Timeline();
    [JsonProperty("user")] public User User { get; set; } = new User();
    //[JsonProperty("ride")] public object Ride { get; set; } = new object();
    [JsonProperty("call")] public Call Call { get; set; } = new Call();
    [JsonProperty("payment_method")] public PaymentMethod PaymentMethod { get; set; } = new PaymentMethod();

}

public class PaymentMethod
{

    [JsonProperty("name")] public string Name { get; set; } = string.Empty;
    [JsonProperty("total")] public float Total { get; set; }
    [JsonProperty("subtotal")] public float Subtotal { get; set; }
    [JsonProperty("change")] public float Change { get; set; }
    [JsonProperty("delivery_tax")] public float DeliveryTax { get; set; }
    [JsonProperty("coupon_value")] public float CouponValue { get; set; }
    [JsonProperty("original_total")] public float OriginalTotal { get; set; }
    [JsonProperty("original_subtotal")] public float OriginalSubtotal { get; set; }
    [JsonProperty("taxable_total")] public float TaxableTotal { get; set; }
    [JsonProperty("service_fee")] public ServiceFee ServiceFee { get; set; } = new ServiceFee();
}

public class Call
{
    //[JsonProperty("last_status")] public object LastStatus { get; set; } = new object();
    [JsonProperty("count")] public int Count { get; set; }
}

public class Store
{
    [JsonProperty("id")] public int Id { get; set; }
    [JsonProperty("name")] public string Name { get; set; } = string.Empty;
    [JsonProperty("phones")] public string Phones { get; set; } = string.Empty;
    [JsonProperty("avatar")] public string Avatar { get; set; } = string.Empty;
    [JsonProperty("business_hours")] public string BusinessHours { get; set; } = string.Empty;
    [JsonProperty("preparation_time")] public int PreparationTime { get; set; }
    [JsonProperty("slug")] public string Slug { get; set; } = string.Empty;
}

public class Timeline
{
    [JsonProperty("created_at")] public string CreatedAt { get; set; } = string.Empty;
    [JsonProperty("read_at")] public object ReadAt { get; set; } = new object();
    //[JsonProperty("in_separation_at")] public object InSeparationAt { get; set; } = new object();
    [JsonProperty("ready_at")] public object ReadyAt { get; set; } = new object();
    //[JsonProperty("cancelled_at")] public object CancelledAt { get; set; } = new object();
  //  [JsonProperty("delivered_at")] public object DeliveredAt { get; set; } = new object();
}

public class User
{
    [JsonProperty("name")] public string Name { get; set; } = string.Empty;
    [JsonProperty("surname")] public string Surname { get; set; } = string.Empty;
}


public class Links
{
    [JsonProperty("first")] public string First { get; set; } = string.Empty;
    [JsonProperty("last")] public string Last { get; set; } = string.Empty;
    [JsonProperty("prev")] public string Prev { get; set; } = string.Empty;
    [JsonProperty("next")] public string Next { get; set; } = string.Empty;
}

public class Meta
{
    [JsonProperty("current_page")] public int CurrentPage { get; set; }
    [JsonProperty("from")] public int From { get; set; }
    [JsonProperty("path")] public string Path { get; set; } = string.Empty;
    [JsonProperty("per_page")] public int PerPage { get; set; }
    [JsonProperty("to")] public int To { get; set; }
}