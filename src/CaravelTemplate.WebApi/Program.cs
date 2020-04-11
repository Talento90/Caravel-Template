using System;
using System.IO;
using System.Threading.Tasks;
using Caravel;
using CaravelTemplate.Infrastructure.Logger;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace CaravelTemplate.WebApi
{
    public static class Program
    {
        private static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile(
                $"appsettings.{Env.GetEnv()}.json",
                optional: true)
            .AddEnvironmentVariables()
            .Build();

        private static IWebHostBuilder CreateWebHostBuilder() =>
            new WebHostBuilder()
                .UseKestrel(options => options.AddServerHeader = false)
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseConfiguration(Configuration)
                .UseStartup<Startup>()
                .UseSerilog();

        public static async Task<int> Main(string[] args)
        {
            Log.Logger = LoggerFactory.CreateLogger(Configuration);

            try
            {
                Log.Information("Starting Application");

                await CreateWebHostBuilder()
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
                Log.CloseAndFlush();
            }
        }
    }
}