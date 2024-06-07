using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SysIntegradorApp.ClassesAuxiliares.logs;

public class Logs
{
    public static async Task CriaLogDeErro(string Error)
    {
        try
        {
            string Data = DateTime.Now.ToString().Substring(0,10).Replace("/", "_");

            string Path = $"C:\\SysLogicaLogs\\logsDeErro_{Data}.txt";

            bool existeArquivo = File.Exists(Path);

            if (!existeArquivo)
            {
                File.Create(Path).Dispose();
            }

            using (FileStream fs = new FileStream(Path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                // Lê o conteúdo existente
                using (StreamReader reader = new StreamReader(fs))
                {
                    //string existingContent = await reader.ReadToEndAsync();

                    // Mover o ponteiro do FileStream para o final do arquivo
                    fs.Seek(0, SeekOrigin.End);

                    using (StreamWriter writer = new StreamWriter(fs))
                    {
                        string ErroAAdicionar = $"****Data do Erro {DateTime.Now}****\n" +
                            $"Erro:\n\n" +
                            $"{Error}" +
                            $"\n\n\n";

                        await writer.WriteLineAsync(ErroAAdicionar);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.Write(ex.ToString());
        }
    }
}
