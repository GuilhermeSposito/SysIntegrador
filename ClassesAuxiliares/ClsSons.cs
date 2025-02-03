using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SysIntegradorApp.ClassesAuxiliares;
internal class ClsSons
{
    private static SoundPlayer? player { get; set; }

    public static void PlaySom()
    {
        //C:\Program Files\SysLogica\SysIntegradorSetup\
        string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string customFolderName = "SysLogicaLogs";

        string fullPath = Path.Combine(appDataPath, customFolderName);

        string path = $"{fullPath}\\SOMS\\UCT-24Horas.wav";

        string caminhoDoArquivo = path;

        if (File.Exists(caminhoDoArquivo))
        {
            player = new SoundPlayer(caminhoDoArquivo);
            player.Play();
        }

    }

    public static void PlaySom2()
    {
        string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string customFolderName = "SysLogicaLogs";

        string fullPath = Path.Combine(appDataPath, customFolderName);

        string path = $"{fullPath}\\SOMS\\IFoodCancel.wav";

        string caminhoDoArquivo = path;

        if (File.Exists(caminhoDoArquivo))
        {
            player = new SoundPlayer(caminhoDoArquivo);
            player.Play();
        }

    }

    public static async Task PlaySomAsync()
    {
        //C:\Program Files\SysLogica\SysIntegradorSetup\
        string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string customFolderName = "SysLogicaLogs";

        string fullPath = Path.Combine(appDataPath, customFolderName);

        string path = $"{fullPath}\\SOMS\\UCT-24Horas.wav";

        string caminhoDoArquivo = path;

        if (File.Exists(caminhoDoArquivo))
        {
            player = new SoundPlayer(caminhoDoArquivo);
            player.Play();
        }

    }  
    
    public static void PlaySomErroDb()
    {
        string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string customFolderName = "SysLogicaLogs";

        string fullPath = Path.Combine(appDataPath, customFolderName);

        string path = $"{fullPath}\\SOMS\\ErroMonitor.wav";

        string caminhoDoArquivo = path;

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

