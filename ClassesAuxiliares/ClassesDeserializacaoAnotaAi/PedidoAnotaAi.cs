using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysIntegradorApp.ClassesAuxiliares.ClassesDeserializacaoAnotaAi;

public class PedidoAnotaAi
{
    [JsonProperty("success")] public bool Success { get; set; }
    [JsonProperty("info")] public InfoDoPedido InfoDoPedido { get; set; } = new InfoDoPedido();
}

public class InfoDoPedido
{
    [JsonProperty("_id")] public string? IdPedido { get; set; }
    [JsonProperty("check")] public int Check { get; set; }
    [JsonProperty("createdAt")] public string? CreatedAt { get; set; }
    [JsonProperty("customer")] public CustomerAnotaAi Customer { get; set; } = new CustomerAnotaAi();
    [JsonProperty("deliveryFee")] public float DeliveryFee { get; set; }
    [JsonProperty("from")] public string? Origem { get; set; }
    [JsonProperty("id")] public string? Id { get; set; }
    [JsonProperty("items")] public List<ItemAnotaAi> Items { get; set; } = new List<ItemAnotaAi>();
    [JsonProperty("menu_version")] public int MenuVersion { get; set; }
    [JsonProperty("observation")] public string? Observation { get; set; }
    [JsonProperty("payments")] public List<Pagamentos> Payments { get; set; } = new List<Pagamentos>();
    [JsonProperty("preparationStartDateTime")] public string? PreparationStartDateTime { get; set; }
    [JsonProperty("salesChannel")] public string? SalesChannel { get; set; }
    [JsonProperty("shortReference")] public int ShortReference { get; set; }
    [JsonProperty("schedule_order")] public ScheduleOrder? Schedule_Order { get; set; }
    [JsonProperty("table")] public string? Table { get; set; }
    [JsonProperty("time_max")] public string? TimeMax { get; set; }
    [JsonProperty("total")] public float Total { get; set; }
    [JsonProperty("type")] public string? Type { get; set; }
    [JsonProperty("updatedAt")] public string? UpdatedAt { get; set; }
    [JsonProperty("waiter_info")] public string? WaiterInfo { get; set; }
    [JsonProperty("discounts")] public List<Descontos> Descontos { get; set; } = new List<Descontos>();
    [JsonProperty("deliveryAddress")] public EnderecoDeDelivery deliveryAddress { get; set; } = new EnderecoDeDelivery();
    [JsonProperty("pdv")] public Pdv Pdv { get; set; } = new Pdv();

}

public class ScheduleOrder
{
    [JsonProperty("date")] public string? Date {  get; set; }
    [JsonProperty("start")] public string? Start {  get; set; }
    [JsonProperty("end")] public string? End {  get; set; }
    [JsonProperty("timezone")] public string? Timezone {  get; set; }
}

public class CustomerAnotaAi
{
    [JsonProperty("id")] public string? id { get; set; }
    [JsonProperty("name")] public string? Nome { get; set; }
    [JsonProperty("phone")] public string? Phone { get; set; }
    [JsonProperty("taxPayerIdentificationNumber")] public string? CPF { get; set; }
}

public class ItemAnotaAi
{
    [JsonProperty("_id")] public string? _Id { get; set; }
    [JsonProperty("id")] public int Id { get; set; }
    [JsonProperty("name")] public string? Name { get; set; }
    [JsonProperty("quantity")] public int quantity { get; set; }
    [JsonProperty("observation")] public string? observation { get; set; }
    [JsonProperty("externalId")] public string? externalCode { get; set; }
    [JsonProperty("internalId")] public string? InternalId { get; set; }
    [JsonProperty("price")] public float Price { get; set; }
    [JsonProperty("total")] public float Total { get; set; }
    [JsonProperty("subItems")] public List<SubItensAnotaAi> SubItens { get; set; } = new List<SubItensAnotaAi>();

}

public class SubItensAnotaAi
{
    [JsonProperty("_id")] public string? _Id { get; set; }
    [JsonProperty("name")] public string? name { get; set; }
    [JsonProperty("quantity")] public int quantity { get; set; }
    [JsonProperty("internalId")] public string? InternalId { get; set; }
    [JsonProperty("price")] public float Price { get; set; }
    [JsonProperty("total")] public float TotalPrice { get; set; }
    [JsonProperty("externalCode")] public string? externalCode { get; set; }
    [JsonProperty("id")] public int Id { get; set; }
    [JsonProperty("id_parent")] public int IdParent { get; set; }

}

public class Pagamentos
{
    [JsonProperty("name")] public string? Nome { get; set; }
    [JsonProperty("code")] public string? Code { get; set; }
    [JsonProperty("value")] public string? value { get; set; }
    [JsonProperty("cardSelected")] public string? CardSelected { get; set; }
    [JsonProperty("externalId")] public string? ExternalId { get; set; }
    [JsonProperty("changeFor")] public double? ChangeFor { get; set; }
    [JsonProperty("prepaid")] public bool Prepaid { get; set; }

}

public class Descontos
{
    [JsonProperty("amount")] public float Total { get; set; }
    [JsonProperty("tag")] public string? Tag { get; set; }

}

public class EnderecoDeDelivery
{
    [JsonProperty("formattedAddress")] public string? FormattedAddress { get; set; }
    [JsonProperty("country")] public string? Country { get; set; }
    [JsonProperty("state")] public string? State { get; set; }
    [JsonProperty("city")] public string? City { get; set; }
    [JsonProperty("neighborhood")] public string? Neighborhood { get; set; }
    [JsonProperty("streetName")] public string? StreetName { get; set; }
    [JsonProperty("streetNumber")] public string? StreetNumber { get; set; }
    [JsonProperty("postalCode")] public string? PostalCode { get; set; }
    [JsonProperty("reference")] public string? Reference { get; set; }
    [JsonProperty("complement")] public string? Complement { get; set; }
}

public class Pdv
{
    [JsonProperty("status")] public bool Status { get; set; }
    [JsonProperty("mode")] public int Mode { get; set; }
    [JsonProperty("table")] public string? Table { get; set; }

}