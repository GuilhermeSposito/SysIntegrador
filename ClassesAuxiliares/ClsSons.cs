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
        //C:\Program Files\SysLogica\SysIntegradorSetup\

        string caminhoDoArquivo = @"C:\Program Files\SysLogica\SysIntegradorSetup\SOMS\UCT-24Horas.wav";

        if (File.Exists(caminhoDoArquivo))
        {
            player = new SoundPlayer(caminhoDoArquivo);
            player.Play();
        }

    }

    public static void PlaySom2()
    {
        //C:\Program Files\SysLogica\SysIntegradorSetup\

        string caminhoDoArquivo = @"C:\Program Files\SysLogica\SysIntegradorSetup\SOMS\IFoodCancel.wav";

        if (File.Exists(caminhoDoArquivo))
        {
            player = new SoundPlayer(caminhoDoArquivo);
            player.Play();
        }

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

