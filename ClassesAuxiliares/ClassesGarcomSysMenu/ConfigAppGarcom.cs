using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysIntegradorApp.ClassesAuxiliares.ClassesGarcomSysMenu;
public class ConfigAppGarcom
{
    [Key][Column("id")] public int Id { get; set; }
    [Column("listadeitens")] public bool ListaDeItens { get; set; }
    [Column("buscadeitens")] public bool BuscaDeItens { get; set; }
    [Column("listaporgrupo")] public bool ListaPorGrupo { get; set; }
    [Column("requisicaoalfanumerica")] public bool RequisicaoAlfaNumerica { get; set; }
    [Column("requisicaonumerica")] public bool RequisicaoNumerica { get; set; }
    [Column("comanda")] public bool Comanda { get; set; }
    [Column("mesa")] public bool Mesa { get; set; }
    [Column("semrequisicao")] public bool SemRequisicao { get; set; }
    [Column("tempoenviopedido")] public int TempoEnvioPedido { get; set; }
    [Column("usabalcao")] public bool UsaBalcao { get; set; }
}
