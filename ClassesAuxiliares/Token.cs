using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    public static string? TokenDaSessao { get; set; }
    public Token(){}
}
