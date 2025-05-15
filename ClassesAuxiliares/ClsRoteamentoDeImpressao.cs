using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysIntegradorApp.ClassesAuxiliares;

public class ClsRoteamentoDeImpressao
{
    [Column("id")] public int Id { get; set; }
    [Column("nome_rota")] public string? NomeRota { get; set; }
    [Column("impressora_caixa")] public string? ImpressoraCaixa { get; set; }
    [Column("impressora_auxiliar")] public string? ImpressoraAuxiliar { get; set; }
    [Column("impressora_cozinha1")] public string? ImpressoraCozinha1 { get; set; }
    [Column("impressora_cozinha2")] public string? ImpressoraCozinha2 { get; set; }
    [Column("impressora_cozinha3")] public string? ImpressoraCozinha3 { get; set; }
    [Column("impressora_bar")] public string? ImpressoraBar { get; set; }
    [Column("ativo")] public bool Ativo { get; set; } = false;

}
