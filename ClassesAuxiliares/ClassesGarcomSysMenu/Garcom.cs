using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysIntegradorApp.ClassesAuxiliares.ClassesGarcomSysMenu;

public class Garcom
{
    [Key][Column("codigo")] public string? Codigo { get; set; }
    [Column("nome")] public string? Nome { get; set; }
    [Column("senha")] public string? Senha { get; set; }
    [Column("valor")] public double Valor { get; set; }
}
