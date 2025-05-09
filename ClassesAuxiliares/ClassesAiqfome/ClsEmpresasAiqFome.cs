using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysIntegradorApp.ClassesAuxiliares.ClassesAiqfome;

public class ClsEmpresasAiqFome
{
    [Column("id")] public int Id { get; set; }
    [Column("client_id")] public string? ClientId { get; set; }
    [Column("nome_identificador")] public string? NomeIdentificador { get; set; }
    [Column("token_req")] public string? TokenReq { get; set; }
    [Column("refresh_token")] public string? RefreshToken { get; set; }
    [Column("token_expiracao")] public string? TokenExpiracao { get; set; }
    [Column("redirect_uri")] public string? RedirectUri { get; set; }
    [Column("code_uri")] public string? CodeUri { get; set; }
    [Column("online")] public bool Online { get; set; } = false;
    [Column("integra_empresa")] public bool IntegraEmpresa { get; set; } = false;
}
