using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.Json.Serialization;


namespace SysIntegradorApp.ClassesAuxiliares;


public class ValidationMessage
{
    [JsonPropertyName("title")]
    public string? Title { get; set; }
    [JsonPropertyName("subtitle")]
    public string? Subtitle { get; set; }
    [JsonPropertyName("description")]
    public string? Description { get; set; }
    [JsonPropertyName("priority")]
    public int Priority { get; set; }

    public ValidationMessage()
    {
        
    }
}

public class Validation
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }
    [JsonPropertyName("code")]
    public string? Code { get; set; }
    [JsonPropertyName("state")]
    public string? State { get; set; }
    [JsonPropertyName("message")]
    public ValidationMessage Message { get; set; } = new ValidationMessage();

    public Validation()
    {
            
    }
}

public class Reopenable
{
    [JsonPropertyName("identifier")]
    public object Identifier { get; set; } = new object();
    [JsonPropertyName("type")]
    public object Type { get; set; } = new object();
    [JsonPropertyName("reopenable")]
    public bool ReopenableFlag { get; set; }

    public Reopenable()
    {
            
    }
}

public class Message
{
    [JsonPropertyName("title")]
    public string? Title { get; set; }
    [JsonPropertyName("subtitle")]
    public string? Subtitle { get; set; }
    [JsonPropertyName("description")]
    public object Description { get; set; } = new object();
    [JsonPropertyName("priority")]
    public object Priority { get; set; } = new Object();

    public Message()
    {
            
    }
}

public class DeliveryStatus
{
    [JsonPropertyName("operation")]
    public string? Operation { get; set; }
    [JsonPropertyName("salesChannel")]
    public string? SalesChannel { get; set; }
    [JsonPropertyName("available")]
    public bool Available { get; set; }
    [JsonPropertyName("state")]
    public string? State { get; set; }
    [JsonPropertyName("reopenable")]
    public Reopenable Reopenable { get; set; } = new Reopenable();
    [JsonPropertyName("validations")]
    public List<Validation> Validations { get; set; } =  new List<Validation>();
    [JsonPropertyName("message")]
    public Message Message { get; set; } = new Message();

    public DeliveryStatus()
    {
            
    }
}

