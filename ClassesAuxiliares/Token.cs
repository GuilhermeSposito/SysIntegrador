using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SysIntegradorApp.ClassesAuxiliares;

[Table("parametrosdeautenticacao")]
public class Token
{
    public int id { get; set; }
    [Column("accesstoken")]public string? accessToken { get; set; }
    [Column("refreshtoken")]public string? refreshToken { get; set; }
    public string? type { get; set; }
    [Column("expiresin")] public int expiresIn { get; set; }
    [Column("venceem")] public string? VenceEm { get; set; }
    [Column("tokendelmatch")] public string? TokenDelMatch { get; set; }
    [Column("venceemdelmatch")] public string? VenceEmDelMatch { get; set; }
    [Column("tokenonpedido")] public string? TokenOnPedido { get; set; }
    [Column("venceemonpedido")] public string? VenceEmOnPedido { get; set; }

    public static string? TokenDaSessao { get; set; }
    public Token(){}
}

public class TokenDelMatchDes
{
    [JsonProperty("success")] public bool Success { get; set; } 
    [JsonProperty("message")] public string? Message { get; set; } 
    [JsonProperty("token")] public string? Token { get; set; } 
    [JsonProperty("expires")] public int Expires { get; set; } 

}

public class TokenDelMatchSessao
{ 
    public static string? Token { get; set; } 
    //public static int Expires { get; set; } 
}