using System;
using System.IO;
using Caravel;
using CaravelTemplate.WebApi.Infrastructure.Data;
using CaravelTemplate.WebApi.Infrastructure.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Sinks.Elasticsearch;

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

        public static int Main(string[] args)
        {
            var logConfig = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .Enrich.With(new RemovePropertiesEnricher())
                .Enrich.FromLogContext()
                .Enrich.WithProperty("app", "mobile-api")
                .Enrich.WithProperty("version", Env.GetAppVersion())
                .Enrich.WithProperty("env", Env.GetEnv());

            logConfig.WriteTo.Elasticsearch(
                new ElasticsearchSinkOptions(new Uri("https://logsene-receiver.eu.sematext.com"))
                {
                    IndexFormat = Environment.GetEnvironmentVariable("LOG_SYSLOG_TAG"),
                    InlineFields = true,
                    AutoRegisterTemplate = true
                });

            Log.Logger = logConfig.CreateLogger();

            try
            {
                Log.Information("Starting Application");
                
                var host = CreateWebHostBuilder().Build();
                
                var dbSettings = Configuration.GetSection("Database").Get<DatabaseSettings>();

                if (!dbSettings.IsInMemory)
                {
                    Log.Information("Applying Database Migration");

                    using var scope = host.Services.CreateScope();
                    var dbContext = scope.ServiceProvider.GetService<CaravelTemplateDbContext>();

                    dbContext.Database.Migrate();
                }

                host.Run();
                
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
        
        private static IWebHostBuilder CreateWebHostBuilder() =>
            new WebHostBuilder()
                .UseKestrel(options => options.AddServerHeader = false)
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseConfiguration(Configuration)
                .UseStartup<Startup>()
                .UseSerilog();
    }
}