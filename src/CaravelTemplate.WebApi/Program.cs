using System;
using System.IO;
using System.Threading.Tasks;
using Caravel;
using CaravelTemplate.Infrastructure.Logger;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace CaravelTemplate.WebApi
{
    public static class Program
    {
        private static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile(
                $"appsettings.{Env.GetEnv().ToLower()}.json",
                optional: true)
            .AddEnvironmentVariables()
            .Build();

        private static IHostBuilder CreateWebHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseKestrel(options => options.AddServerHeader = false)
                        .UseConfiguration(Configuration)
                        .UseStartup<Startup>();
                })
                .UseSerilog();

        public static async Task<int> Main(string[] args)
        {
            Log.Logger = LoggerFactory.CreateLogger(Configuration);

            try
            {
                Log.Information("Starting Application");

                await CreateWebHostBuilder(args)
                    .Build()
                    .RunAsync();

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
    }
}