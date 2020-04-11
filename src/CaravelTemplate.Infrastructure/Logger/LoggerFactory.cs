using Caravel;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace CaravelTemplate.Infrastructure.Logger
{
    public static class LoggerFactory
    {
        public static Serilog.Core.Logger CreateLogger(IConfiguration  configuration)
        {
            var logConfig = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("app", "CaravelTemplate")
                .Enrich.WithProperty("version", Env.GetAppVersion())
                .Enrich.WithProperty("env", Env.GetEnv());
            
            return logConfig.CreateLogger();
        }
    }
}