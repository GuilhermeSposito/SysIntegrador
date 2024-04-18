using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;

namespace SysIntegradorApp.ClassesAuxiliares;
internal class ClsSons
{
    private static SoundPlayer? player { get; set; }

    public static void PlaySom()
    {
        player = new SoundPlayer("C:\\Users\\gui-c\\OneDrive\\Área de Trabalho\\SysIntegrador\\SysIntegradorApp\\SOM\\UCT-24Horas.wav");
        player.Play();
    }

    public static void StopSom()
    {
        if (player != null)
        {
            player.Stop();
            player = null;
        }
    }
}

