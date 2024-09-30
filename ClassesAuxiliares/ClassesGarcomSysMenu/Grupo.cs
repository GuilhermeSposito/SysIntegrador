using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysIntegradorApp.ClassesAuxiliares.ClassesGarcomSysMenu
{
    public class Grupo
    {
        [Key][Column("codigo")] public string? Codigo { get; set; }
        [Column("descricao")] public string? Descricao { get; set; }
        [Column("familia")] public string? Familia { get; set; }
        [Column("ocultatablet")] public bool Oculta { get; set; }
        [Column("totgrupo")] public double TOTGRUPO { get; set; }
    }
}
