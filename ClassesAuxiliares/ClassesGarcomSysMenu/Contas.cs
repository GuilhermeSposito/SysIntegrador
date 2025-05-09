using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysIntegradorApp.ClassesAuxiliares.ClassesGarcomSysMenu;

public class Contas
{
    [Key][Column("id")] public int Id { get; set; }
    [Column("conta")] public string? Conta { get; set; }
    [Column("mesa")] public string? Mesa { get; set; }
    [Column("qtdade")] public float Qtdade { get; set; }
    [Column("codcarda1")] public string? CodCarda1 { get; set; }
    [Column("codcarda2")] public string? CodCarda2 { get; set; }
    [Column("codcarda3")] public string? CodCarda3 { get; set; }
    [Column("tamanho")] public string? Tamanho { get; set; }
    [Column("descarda")] public string? Descarda { get; set; }
    [Column("valorunit")] public string? ValorUnit { get; set; }
    [Column("valortotal")] public string? ValorTotal { get; set; }
    [Column("datainicio")] public string? DataInicio { get; set; }
    [Column("horainicio")] public string? HoraInicio { get; set; }
    [Column("obs1")] public string? Obs1 { get; set; }
    [Column("obs2")] public string? Obs2 { get; set; }
    [Column("obs3")] public string? Obs3 { get; set; }
    [Column("obs4")] public string? Obs4 { get; set; }
    [Column("obs5")] public string? Obs5 { get; set; }
    [Column("obs6")] public string? Obs6 { get; set; }
    [Column("obs7")] public string? Obs7 { get; set; }
    [Column("obs8")] public string? Obs8 { get; set; }
    [Column("obs9")] public string? Obs9 { get; set; }
    [Column("obs10")] public string? Obs10 { get; set; }
    [Column("obs11")] public string? Obs11 { get; set; }
    [Column("obs12")] public string? Obs12 { get; set; }
    [Column("obs13")] public string? Obs13 { get; set; }
    [Column("obs14")] public string? Obs14 { get; set; }
    [Column("obs15")] public string? Obs15 { get; set; }
    [Column("cliente")] public string? Cliente { get; set; }
    [Column("requisicao")] public string? Requisicao { get; set; }
    [Column("status")] public string? Status { get; set; }
    [Column("telefone")] public string? Telefone { get; set; }
    [Column("impcomanda")] public string? ImpComanda { get; set; }
    [Column("impcomanda2")] public string? ImpComanda2 { get; set; }
    [Column("qtdcomanda")] public float QtdComanda { get; set; }
    [Column("usuario")] public string? Usuario { get; set; }
}
