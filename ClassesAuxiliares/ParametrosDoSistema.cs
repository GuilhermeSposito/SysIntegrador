using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace SysIntegradorApp.ClassesAuxiliares;

public class ParametrosDoSistema
{
    [Column("id")]
    public int Id { get; set; }  
    [Column("nomefantasia")]
    public string? NomeFantasia { get; set; }
    [Column("endereco")]
    public string? Endereco { get; set; }
    [Column("impressaoaut")]
    public bool ImpressaoAut { get; set; }
    [Column("aceitapedidoaut")]
    public bool AceitaPedidoAut { get; set; }
    [Column("caminhodobanco")]
    public string? CaminhodoBanco { get; set; }
    [Column("caminhoservidor")]
    public string? CaminhoServidor { get; set; }
    [Column("integracaosysmenu")]
    public bool IntegracaoSysMenu { get; set; }
    [Column("impressora1")]
    public string? Impressora1 { get; set; }
    [Column("impressora2")]
    public string? Impressora2 { get; set; }
    [Column("impressora3")]
    public string? Impressora3 { get; set; }
    [Column("impressora4")]
    public string? Impressora4 { get; set; }
    [Column("impressora5")]
    public string? Impressora5 { get; set; }
    [Column("telefone")]
    public string? Telefone { get; set; }
    [Column("clientid")]
    public string? ClientId { get; set; }
    [Column("clientsecret")]

    public string? ClientSecret { get; set; }

    public ParametrosDoSistema() {}
}
