using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysIntegradorApp.ClassesAuxiliares;

public class EmpresasEntregaTaxyMachine
{
    [Column("id")] public int Id { get; set; }
    [Column("nome_empresa")] public string? NomeEmpresa { get; set; }
    [Column("usuario")] public string? Usuario { get; set; }
    [Column("senha")] public string? Senha { get; set; }
    [Column("machine_id")] public string? MachineId { get; set; }
    [Column("integra")] public bool Integra { get; set; } = false;
    [Column("cod_entregador")] public string? CodEntregador { get; set; }
    [Column("tipo_pagamento")] public string? TipoPagamento { get; set; }
}
