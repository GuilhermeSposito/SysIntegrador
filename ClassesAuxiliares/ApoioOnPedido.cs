using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysIntegradorApp.ClassesAuxiliares;

[Table("apoioonpedido")]
public class ApoioOnPedido
{ 
    [Column("id")] public int Id { get; set; }
    [Column("id_pedido")] public int Id_Pedido { get; set; }
    [Column("action")] public string? Action {  get; set; } 
    public ApoioOnPedido(){}
}
