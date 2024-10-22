using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SysIntegradorApp.ClassesDeConexaoComApps;
using SysIntegradorApp.data;
using SysIntegradorApp.data.InterfaceDeContexto;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Windows.Forms.Design;

namespace SysIntegradorApp
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            string nomeDoProcesso = "SysIntegradorApp";

            Process[] processos = Process.GetProcessesByName(nomeDoProcesso);

            if (processos.Length > 1)
            {
                MessageBox.Show("O aplicativo já está em execução.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                return;
            }

            MigrationManager.ApplyMigrations(new ApplicationDbContext());

            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(serviceProvider.GetRequiredService<Form1>());
            serviceProvider.GetRequiredService<FormMenuInicial>();
            serviceProvider.GetRequiredService<Ifood>();
            serviceProvider.GetRequiredService<CCM>();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql("Host=localhost;Port=5432;Database=integrador;Username=postgres;Password=69063360"));

            services.AddScoped<FormMenuInicial>();
            services.AddScoped<Ifood>();
            services.AddScoped<OnPedido>();
            services.AddScoped<CCM>();
            services.AddScoped<IMeuContexto, MeuContexto>();
            services.AddScoped<Form1>();
        }
    }
}