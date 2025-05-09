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
    [NotMapped] public string? Codigo2 { get; set; }
    [NotMapped] public string? Codigo3 { get; set; }
    [NotMapped] public Guid IdInterno { get; set; } = Guid.NewGuid();
    [Column("descricao")] public string? Descricao { get; set; }
    [Column("grupo")] public string? Grupo { get; set; }
    [Column("fracionado")] public string? Fracionado { get; set; }
    [Column("tamanhounico")] public string? TamanhoUnico { get; set; }
    [NotMapped] public string? Tamanho { get; set; }
    [Column("pvenda1", TypeName = "decimal(10,2)")] public double Preco1 { get; set; } = 0;
    [Column("pvenda2", TypeName = "decimal(10,2)")] public double Preco2 { get; set; } = 0;
    [Column("pvenda3", TypeName = "decimal(10,2)")] public double Preco3 { get; set; } = 0;
    [Column("ocultatablet")] public bool OcultaTablet { get; set; }
    [NotMapped] public string? Observacao { get; set; } = String.Empty;
    [NotMapped] public string? Requisicao { get; set; } = String.Empty;
    [NotMapped] public float Quantidade { get; set; } = 1;
    [NotMapped] public List<Incremento> incrementos { get; set; } = new List<Incremento>();
}
