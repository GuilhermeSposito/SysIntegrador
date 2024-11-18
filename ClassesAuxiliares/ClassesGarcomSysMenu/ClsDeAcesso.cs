using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysIntegradorApp.ClassesAuxiliares.ClassesGarcomSysMenu;

public class ClsDeAcesso
{
    [Key][Column("id")] public int Id { get; set; }
    [Column("usuario")] public string? Usuario { get; set; }
    [Column("senha")] public string? Senha { get; set; }
}