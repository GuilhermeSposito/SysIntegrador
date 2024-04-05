using System;
using System.Collections.Generic;
//using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SysIntegradorApp.ClassesAuxiliares;
using System.Data.OleDb;

namespace SysIntegradorApp;

public class ApplicationDbContext : DbContext
{
    public DbSet<Token> parametrosdeautenticacao {  get; set; }
    //public DbSet<Pulling> pulling { get; set; }
    public DbSet<ParametrosDoPedido> parametrosdopedido { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=ifood;Username=postgres;Password=69063360");
        }
    }
}


