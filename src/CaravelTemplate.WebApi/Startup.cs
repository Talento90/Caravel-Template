using System.Text.Json;
using System.Text.Json.Serialization;
using AutoMapper;
using FluentValidation.AspNetCore;
using Caravel;
using Caravel.AppContext;
using Caravel.AspNetCore.Extensions;
using Caravel.AspNetCore.Filters;
using Caravel.AspNetCore.Http;
using Caravel.AspNetCore.Middleware;
using CaravelTemplate.WebApi.Extensions;
using CaravelTemplate.WebApi.Infrastructure.Data;
using CaravelTemplate.WebApi.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CaravelTemplate.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                .AddFluentValidation()
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddMvcOptions(opt => { opt.Filters.Add(new ValidateModelFilter()); })
                .AddJsonOptions(opt =>
                {
                    opt.JsonSerializerOptions.IgnoreNullValues = true;
                    opt.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
                    opt.JsonSerializerOptions.Converters.Add(new JsonDateTimeConverter());
                });

            services.ConfigureEntityFramework(Configuration);
            services.ConfigureSwagger();
            services.ConfigureAuthentication(Configuration);
            services.AddRouting(r => r.LowercaseUrls = true);
            services.Configure<ApiBehaviorOptions>(opt => { opt.SuppressModelStateInvalidFilter = true; });

            services.AddHttpContextAccessor();
            services.AddAutoMapper(typeof(Startup).Assembly);
            services.AddScoped<IAppContextAccessor, AppContextAccessor>();

            // Register Business Services
            services.AddScoped<DeviceService>();

            services
                .AddHealthChecks()
                .AddDbContextCheck<CaravelTemplateDbContext>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseHttpsRedirection();
            
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMiddleware<TraceIdMiddleware>();
            app.UseMiddleware<AppContextEnricherMiddleware>();
            app.UseMiddleware<LoggingMiddleware>();
            app.UseMiddleware<ExceptionMiddleware>();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
                endpoints.MapServiceVersion("/api/version");
            });

            if (!Env.IsDevelopmentEnvironment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options => { options.SwaggerEndpoint("/swagger/v1/swagger.json", "Caravel API"); });
            }
        }
    }
}