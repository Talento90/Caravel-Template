using System.Text.Json;
using System.Text.Json.Serialization;
using AutoMapper;
using FluentValidation.AspNetCore;
using Caravel.AppContext;
using Caravel.AspNetCore.Middleware;
using Caravel.Http;
using CaravelTemplate.Core.Behaviours;
using CaravelTemplate.Core.Books.Queries;
using CaravelTemplate.Infrastructure.Data;
using CaravelTemplate.WebApi.Extensions;
using MediatR;
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
                .AddFluentValidation(fv => fv.RegisterValidatorsFromAssembly(typeof(GetBookByIdQuery).Assembly))
                .AddJsonOptions(opt =>
                {
                    opt.JsonSerializerOptions.IgnoreNullValues = true;
                    opt.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
                    opt.JsonSerializerOptions.Converters.Add(new JsonDateTimeConverter());
                });

            services.Configure<ApiBehaviorOptions>(opt => { opt.SuppressModelStateInvalidFilter = true; });

            services.ConfigureEntityFramework(Configuration);
            services.ConfigureSwagger();
            services.AddRouting(r => r.LowercaseUrls = true);

            services.AddAutoMapper(typeof(GetBookByIdQuery).Assembly);
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPerformanceBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));

            services.AddMediatR(typeof(GetBookByIdQuery).Assembly);

            services.AddHttpContextAccessor();
            services.AddScoped<IAppContextAccessor, AppContextAccessor>();

            services
                .AddHealthChecks()
                .AddDbContextCheck<CaravelTemplateDbContext>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseHealthChecks("/health");
            app.UseAppVersion("/api/version");

            app.UseMiddleware<TraceIdMiddleware>();
            app.UseMiddleware<AppContextEnricherMiddleware>();
            app.UseMiddleware<LoggingMiddleware>(Options.Create(new LoggingOptions
            {
                EnableLogBody = true
            }));
            app.UseMiddleware<ExceptionMiddleware>();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "CaravelTemplate API");
            });
        }
    }
}