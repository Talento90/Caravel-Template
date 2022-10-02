using System;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using Caravel.ApplicationContext;
using CaravelTemplate.Identity;
using CaravelTemplate.Identity.Data;
using CaravelTemplate.Infrastructure.Data;
using CaravelTemplate.Infrastructure.Logger;
using CaravelTemplate.WebApi.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace CaravelTemplate.SetupConsole
{
    internal static class Program
    {
        private static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Env.GetEnv()}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        public static async Task<int> Main(string[] args)
        {
            Log.Logger = LoggerFactory.CreateLogger(Configuration);

            try
            {
                Log.Information("Setup Application");

                var host = CreateHostBuilder()
                    .Build();
                
                var dbContext = host.Services.GetService<CaravelTemplateTemplateDbContext>() ?? throw new NoNullAllowedException();
                var identityDbContext = host.Services.GetService<CaravelTemplateIdentityDbContext>() ?? throw new NoNullAllowedException();

                Log.Information("Apply Database Migrations");

                await dbContext.Database.MigrateAsync();
                await identityDbContext.Database.MigrateAsync();
                
                Log.Information("Setup Completed");

                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, ex.Message);

                return 1;
            }
            finally
            {
                await Log.CloseAndFlushAsync();
            }
        }

        private static IHostBuilder CreateHostBuilder() =>
            new HostBuilder()
                .ConfigureAppConfiguration(c => { })
                .ConfigureLogging(((context, builder) => { builder.AddSerilog(); }))
                .ConfigureServices(services =>
                {
                    services.AddTransient<IAppContextAccessor, AppContextAccessor>();
                    services.ConfigureEntityFramework(Configuration);
                })
                .UseSerilog();
    }
}