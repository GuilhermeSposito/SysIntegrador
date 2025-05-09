using System;
using System.Collections.Generic;
//using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SysIntegradorApp.ClassesAuxiliares;
using System.Data.OleDb;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using SysIntegradorApp.ClassesAuxiliares.ClassesGarcomSysMenu;
using SysIntegradorApp.ClassesAuxiliares.Ifood;
using SysIntegradorApp.ClassesAuxiliares.ClassesAiqfome;

namespace SysIntegradorApp.data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public ApplicationDbContext()
    {

    }

    public DbSet<Token> parametrosdeautenticacao { get; set; }
    public DbSet<ParametrosDoPedido> parametrosdopedido { get; set; }
    public DbSet<ParametrosDoSistema> parametrosdosistema { get; set; }
    public DbSet<ApoioOnPedido> apoioonpedido { get; set; }
    public DbSet<Garcom> garcons { get; set; }
    public DbSet<Produto> cardapio { get; set; }
    public DbSet<Mesa> mesas { get; set; }
    public DbSet<Grupo> grupos { get; set; }
    public DbSet<Contas> contas{ get; set; }
    public DbSet<Incremento> incrementos { get; set; }
    public DbSet<IncrementoCardapio> incrementocardapio { get; set; }
    public DbSet<ConfigAppGarcom> configappgarcom { get; set; }
    public DbSet<ApoioAppGarcom> apoioappgarcom { get; set; }
    public DbSet<ClsDeAcesso> acesso { get; set; }
    public DbSet<Promocoes> promocoes { get; set; }
    public DbSet<Setup> setup { get; set; }
    public DbSet<EmpresasIfood> empresasIfoods { get; set; }
    public DbSet<ClsEmpresasAiqFome> empresasaiqfome { get; set; }
    public DbSet<EmpresasEntregaTaxyMachine> empresastaxymachine { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=integrador;Username=postgres;Password=69063360");
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


public static class MigrationManager
{
    public static void ApplyMigrations(ApplicationDbContext dbContext)
    {
        // Cria e aplica migrações pendentes
        var migrator = dbContext.GetService<IMigrator>();
        var pendingMigrations = dbContext.Database.GetPendingMigrations();

        if (pendingMigrations.Any())
        {
            foreach (var migration in pendingMigrations)
            {
                migrator.Migrate(migration);
            }
        }
    }
}