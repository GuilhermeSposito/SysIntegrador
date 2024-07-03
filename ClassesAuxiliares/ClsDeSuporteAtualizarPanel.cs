using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysIntegradorApp.ClassesAuxiliares;

public static class ClsDeSuporteAtualizarPanel
{
    public static bool MudouDataBase { get; set; } = false;
    public static bool MudouDataBasePedido { get; set; } = false;
}

public static class ClsSuporteDePedidoNaoEnviadoDelmatch
{
    public static bool ErroDeEnvioDePedido { get; set; } = false;
    public static List<string> PedidosQueNaoForamEnviados { get; set; } = new List<string>();
    public static List<string> MotivosDeNaoTerEnviado { get; set; } = new List<string>();
}