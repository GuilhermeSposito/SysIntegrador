using SysIntegradorApp.ClassesAuxiliares.logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysIntegradorApp.data.InterfaceDeContexto;

public class MeuContexto : IMeuContexto
{
    public async Task<ApplicationDbContext> GetContextoAsync()
    {
        try
        {
            return new ApplicationDbContext();

        }
        catch (Exception ex)
        {
            await Logs.CriaLogDeErro(ex.Message);
        }
        return new ApplicationDbContext();
    }
}
