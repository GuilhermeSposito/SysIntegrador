using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysIntegradorApp.ClassesAuxiliares.ClassesGarcomSysMenu;

public class Produto
{
    [Key][Column("codigo")] public string? Codigo { get; set; }
    [Column("descricao")] public string? Descricao { get; set; }
    [Column("grupo")] public string? Grupo { get; set; }
    [Column("fracionado")] public string? Fracionado { get; set; }
    [Column("tamanhounico")] public string? TamanhoUnico { get; set; }
    [Column("pvenda1",TypeName = "decimal(10,2)")] public double Preco1 { get; set; }
    [Column("pvenda2", TypeName = "decimal(10,2)")] public double Preco2 { get; set; }
    [Column("pvenda3", TypeName = "decimal(10,2)")] public double Preco3 { get; set; }
    [Column("ocultatablet")] public bool OcultaTablet { get; set; }
}
