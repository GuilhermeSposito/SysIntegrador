using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysIntegradorApp.ClassesAuxiliares;



[Table("parametrosdopedido")]
public class ParametrosDoPedido
{
    [Column("id")] public string? Id { get; set; }
    [Column("json")] public string? Json { get; set; }
    [Column("situacao")] public string? Situacao { get; set; }
    [Column("conta")] public int Conta { get; set; }
    [Column("criadoem")] public string CriadoEm { get; set; }
    [Column("displayid")] public int DisplayId {  get; set; }  
    [Column("jsonpolling")] public string JsonPolling {  get; set; }  
    
    public ParametrosDoPedido() { }


}
