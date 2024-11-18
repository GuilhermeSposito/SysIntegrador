using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysIntegradorApp.ClassesAuxiliares.ClassesGarcomSysMenu;

public class ApoioAppGarcom
{
    [Key][Column("id")] public int Id { get; set; }
    [Column("pedidojson")] public string? PedidoJson { get; set; }
    [Column("obs")] public string? Obs { get; set; }
    [Column("criadoem")] public DateTime CriadoEm { get; set; } = DateTime.Now;
    [Column("processado")] public bool Processado { get; set; }
    [Column("tipo")] public string? Tipo { get; set; }

}
