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
    public ParametrosDoPedido() { }

    public ParametrosDoPedido(string id, string json, string situacao, int conta)
    {
        Id = id;
        Json = json;
        Situacao = situacao;
        Conta = conta;
    }
}
