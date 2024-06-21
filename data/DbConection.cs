using System;
using System.Collections.Generic;
//using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SysIntegradorApp.ClassesAuxiliares;
using System.Data.OleDb;

namespace SysIntegradorApp.data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
           : base(options)
    {
    }

    public ApplicationDbContext()
    {
            
    }

    public DbSet<Token> parametrosdeautenticacao { get; set; }
    public DbSet<ParametrosDoPedido> parametrosdopedido { get; set; }
    public DbSet<ParametrosDoSistema> parametrosdosistema {  get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=ifood;Username=postgres;Password=69063360");
        }
    }

    public static string RetornaCaminhoBaseSysMenu()
    {
        try
        {
            using ApplicationDbContext db = new ApplicationDbContext();

            var opcSistema = db.parametrosdosistema.Where(x => x.Id == 1).FirstOrDefault();

            return opcSistema.CaminhodoBanco;
        }
        catch (Exception ex)
        {
            MessageBox.Show("Erro ao procurar base", "Ops");
        }
        return "";
    }
}


