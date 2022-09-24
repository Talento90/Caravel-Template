using Caravel.AspNetCore.Middleware;
using CaravelTemplate.Infrastructure.Data;
using CaravelTemplate.WebApi.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CaravelTemplate.WebApi
{
    public class Startup
    {
        private IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    var settings = Caravel.Http.JsonSerializerOptions.CamelCase();
                    options.SerializerSettings.ContractResolver = settings.ContractResolver;
                    options.SerializerSettings.NullValueHandling = settings.NullValueHandling;
                    options.SerializerSettings.Converters = settings.Converters;
                    options.SerializerSettings.DateFormatString = settings.DateFormatString;
                    options.SerializerSettings.DateTimeZoneHandling = settings.DateTimeZoneHandling;
                    options.SerializerSettings.DateFormatHandling = settings.DateFormatHandling;
                });
            
            services.Configure<ApiBehaviorOptions>(opt => { opt.SuppressModelStateInvalidFilter = true; });

            services.ConfigureEntityFramework(Configuration);
            services.ConfigureAuthentication(Configuration);
            services.ConfigureSwagger();
            services.ConfigureServices();
            services.ConfigureObservability();

            services.AddRouting(r => r.LowercaseUrls = true);
            services
                .AddHealthChecks()
                .AddDbContextCheck<CaravelTemplateTemplateDbContext>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseHealthChecks("/health");
            app.UseAppVersion("/api/version");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMiddleware<ExceptionMiddleware>();
            app.UseMiddleware<TraceIdMiddleware>();
            app.UseMiddleware<AppContextEnricherMiddleware>();
            app.UseMiddleware<LoggingMiddleware>(Options.Create(new LoggingSettings()
            {
                EnableLogBody = false
            }));
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapPrometheusScrapingEndpoint("/metrics");
            });

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "CaravelTemplate API");
            });
        }
    }
}