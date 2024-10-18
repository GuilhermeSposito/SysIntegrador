using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoDelmatch;

namespace SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoOnPedido;

public class PedidoOnPedido
{
    [JsonProperty("status")] public bool Status { get; set; }
    [JsonProperty("return")] public Return Return { get; set; } = new Return();

    public PedidoOnPedido() { }
}

public class PedidoOnPedido2
{
    [JsonProperty("status")] public bool Status { get; set; }
    [JsonProperty("return")] public List<Return> Return { get; set; } = new List<Return>();

    public PedidoOnPedido2() { }
}

public class Return
{
    [JsonProperty("id")] public string? Id { get; set; }
    [JsonProperty("type")] public string? Type { get; set; }
    [JsonProperty("displayId")] public string? DisplayId { get; set; }
    [JsonProperty("sourceAppId")] public string? SourceAppId { get; set; }
    [JsonProperty("createdAt")] public string? CreatedAt { get; set; }
    [JsonProperty("lastEvent")] public string? LastEvent { get; set; }
    [JsonProperty("orderTiming")] public string? OrderTiming { get; set; }
    [JsonProperty("preparationStartDateTime")] public string? PreparationStartDateTime { get; set; }
    [JsonProperty("merchant")] public Merchant Merchant { get; set; } = new Merchant();
    [JsonProperty("items")] public List<itemsOn> ItemsOn { get; set; } = new List<itemsOn>();
    [JsonProperty("otherFees")] public List<OtherFees> OtherFees { get; set; } = new List<OtherFees>();
    [JsonProperty("discounts")] public List<Discounts> Discounts { get; set; } = new List<Discounts>();
    [JsonProperty("total")] public Total Total { get; set; } = new Total();
    [JsonProperty("Payments")] public Payments Payments { get; set; } = new Payments();
    [JsonProperty("customer")] public Customer Customer { get; set; } = new Customer();
    [JsonProperty("schedule")] public Schedule Schedule { get; set; } = new Schedule();
    [JsonProperty("delivery")] public DeliveryOn Delivery { get; set; } = new DeliveryOn();
    [JsonProperty("takeout")] public TakeOut TakeOut { get; set; } = new TakeOut();
    [JsonProperty("indoor")] public Indoor Indoor { get; set; } = new Indoor();
    [JsonProperty("extraInfo")] public List<ExtraInfo> extraInfo { get; set; } = new List<ExtraInfo>();

    public Return() { }
}

public class Merchant
{
    [JsonProperty("id")] public string? Id { get; set; }
    [JsonProperty("name")] public string? name { get; set; }

    public Merchant() { }
}

public class itemsOn
{
    [JsonProperty("id")] public string? Id { get; set; }
    [JsonProperty("index")] public int Index { get; set; }
    [JsonProperty("name")] public string? Name { get; set; }
    [JsonProperty("unit")] public string? Unit { get; set; }
    [JsonProperty("externalCode")] public string? externalCode { get; set; }
    [JsonProperty("ean")] public string? Ean { get; set; }
    [JsonProperty("quantity")] public int quantity { get; set; }
    [JsonProperty("specialInstructions")] public string? observations { get; set; }
    [JsonProperty("unitPrice")] public UnitPrice UnitPrice { get; set; } = new UnitPrice();
    [JsonProperty("optionsPrice")] public OptionsPrice OptionsPrice { get; set; } = new OptionsPrice();
    [JsonProperty("totalPrice")] public TotalPrice TotalPrice { get; set; } = new TotalPrice();
    [JsonProperty("options")] public List<Options> Options { get; set; } = new List<Options>();
   

    public itemsOn() { }
}

public class UnitPrice
{
    [JsonProperty("value")] public float Value { get; set; }
    [JsonProperty("currency")] public string? Currency { get; set; }

    public UnitPrice() { }
}

public class OptionsPrice
{
    [JsonProperty("value")] public float Value { get; set; }
    [JsonProperty("currency")] public string? Currency { get; set; }

    public OptionsPrice() { }
}

public class TotalPrice
{
    [JsonProperty("value")] public float Value { get; set; }
    [JsonProperty("currency")] public string? Currency { get; set; }

    public TotalPrice() { }
}

public class Options
{
    [JsonProperty("index")] public int Index { get; set; }
    [JsonProperty("id")] public int Id { get; set; }
    [JsonProperty("name")] public string? name { get; set; }
    [JsonProperty("externalCode")] public string? externalCode { get; set; }
    [JsonProperty("unit")] public string? Unit { get; set; }
    [JsonProperty("ean")] public string? Ean { get; set; }
    [JsonProperty("quantity")] public int quantity { get; set; }
    [JsonProperty("unitPrice")] public UnitPrice UnitPrice { get; set; } = new UnitPrice();
    [JsonProperty("totalPrice")] public TotalPrice TotalPrice { get; set; } = new TotalPrice();
    [JsonProperty("specialInstructions")] public string? specialInstructions { get; set; }


    public Options() { }
}

public class OtherFees
{
    [JsonProperty("type")] public string? Type { get; set; }
    [JsonProperty("name")] public string? Name { get; set; }
    [JsonProperty("receivedBy")] public string? ReceivedBy { get; set; }
    [JsonProperty("observation")] public string? Observation { get; set; }
    [JsonProperty("price")] public Price? Price { get; set; } = new Price();


    public OtherFees() { }
}

public class Price
{
    [JsonProperty("value")] public float Value { get; set; }
    [JsonProperty("currency")] public string? Currency { get; set; }

    public Price() { }
}

public class Discounts
{
    [JsonProperty("amount")] public Amount Amount { get; set; } = new Amount();
    [JsonProperty("target")] public string? target { get; set; }
    [JsonProperty("targetId")] public string? TargetId { get; set; }
    [JsonProperty("sponsorshipValues")] public List<SponsorshipValues> SponsorshipValues { get; set; } = new List<SponsorshipValues>();

    public Discounts() { }
}

public class Amount
{
    [JsonProperty("value")] public float value { get; set; }
    [JsonProperty("currency")] public string? Currency { get; set; }

    public Amount() { }
}

public class SponsorshipValues
{
    [JsonProperty("name")] public string? Name { get; set; }
    [JsonProperty("amount")] public Amount Amount { get; set; } = new Amount();

    public SponsorshipValues() { }
}

public class Total
{
    [JsonProperty("itemsPrice")] public ItemsPrice ItemsPrice { get; set; } = new ItemsPrice();
    [JsonProperty("otherFees")] public OtherFees OtherFees { get; set; } = new OtherFees();
    [JsonProperty("discount")] public Discounts Discounts { get; set; } = new Discounts();
    [JsonProperty("orderAmount")] public OrderAmount OrderAmount { get; set; } = new OrderAmount();

    public Total() { }
}

public class ItemsPrice
{
    [JsonProperty("value")] public float value { get; set; }
    [JsonProperty("currency")] public string? Currency { get; set; }

    public ItemsPrice() { }
}

public class OrderAmount
{
    [JsonProperty("value")] public float value { get; set; }
    [JsonProperty("currency")] public string? Currency { get; set; }
    public OrderAmount() { }
}

public class Payments
{
    [JsonProperty("prepaid")] public float Prepaid { get; set; }
    [JsonProperty("pending")] public float Pending { get; set; }
    [JsonProperty("methods")] public List<MethodsOn> Methods { get; set; } = new List<MethodsOn>();

    public Payments() { }
}

public class MethodsOn
{
    [JsonProperty("value")] public float value { get; set; }
    [JsonProperty("currency")] public string? Currency { get; set; }
    [JsonProperty("type")] public string? Type { get; set; }
    [JsonProperty("method")] public string? Method { get; set; }
    [JsonProperty("brand")] public string? Brand { get; set; }
    [JsonProperty("methodInfo")] public string? MethodInfo { get; set; }
    [JsonProperty("changeFor")] public float ChangeFor { get; set; }

    public MethodsOn() { }
}

public class Customer
{
    [JsonProperty("id")] public string? Id { get; set; }
    [JsonProperty("phone")] public PhoneOn? PhoneOn { get; set; } = new PhoneOn();
    [JsonProperty("documentNumber")] public string? DocumentNumber { get; set; }
    [JsonProperty("name")] public string? Name { get; set; }
    [JsonProperty("ordersCountOnMerchant")] public int? OrdersCountOnMerchant { get; set; }

    public Customer() { }
}

public class PhoneOn
{
    [JsonProperty("number")] public string? Number { get; set; }
    [JsonProperty("extension")] public string? Extension { get; set; }

    public PhoneOn(){ }
}

public class Schedule
{
    [JsonProperty("scheduledDateTimeStart")] public string? ScheduledDateTimeStart { get; set; }
    [JsonProperty("scheduledDateTimeEnd")] public string? ScheduledDateTimeEnd { get; set; }

    public Schedule() { }
}

public class DeliveryOn
{
    [JsonProperty("deliveredBy")] public string? DeliveredBy { get; set; }
    [JsonProperty("deliveryAddress")] public DeliveryAddressON DeliveryAddressON { get; set; } = new DeliveryAddressON();
    [JsonProperty("estimatedDeliveryDateTime")] public string? EstimatedDeliveryDateTime { get; set; }
    [JsonProperty("deliveryDateTime")] public string? DeliveryDateTime { get; set; }

    public DeliveryOn() { }
}

public class DeliveryAddressON
{
    [JsonProperty("country")] public string? Country { get; set; }
    [JsonProperty("state")] public string? State { get; set; }
    [JsonProperty("city")] public string? City { get; set; }
    [JsonProperty("district")] public string? District { get; set; }
    [JsonProperty("street")] public string? Street { get; set; }
    [JsonProperty("number")] public string? Number { get; set; }
    [JsonProperty("complement")] public string? Complement { get; set; }
    [JsonProperty("reference")] public string? Reference { get; set; }
    [JsonProperty("formattedAddress")] public string? FormattedAddress { get; set; }
    [JsonProperty("postalCode")] public string? PostalCode { get; set; }

    public DeliveryAddressON() { }
}

public class TakeOut
{
    [JsonProperty("mode")] public string? Mode { get; set; }  
    [JsonProperty("takeoutDateTime")] public string? TakeoutDateTime { get; set; }  
    public TakeOut(){}
}

public class Indoor
{
    [JsonProperty("place")] public string? Place { get; set; }
    [JsonProperty("indoorDateTime")] public string? IndoorDateTime { get; set; }
    [JsonProperty("mode")] public string? Mode { get; set; }

    public Indoor() { }
}

public class ExtraInfo
{
    public ExtraInfo() { }
}