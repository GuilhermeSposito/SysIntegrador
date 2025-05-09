using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysIntegradorApp.ClassesAuxiliares.Ifood;

public class EmpresasIfood
{
    [Key][Column("id")]public int Id { get; set; }
    [Column("nome")] public string? NomeIdentificador { get; set; }
    [Column("merchantid")] public string? MerchantId { get; set; }
    [Column("token")] public string? Token { get; set; }
    [Column("refreshToken")] public string? RefreshToken { get; set; }
    [Column("dataExpiracao")] public string? DataExpiracao { get; set; }
    [Column("integraempresa")] public bool IntegraEmpresa { get; set; }
    [Column("online")] public bool Online { get; set; }
}
