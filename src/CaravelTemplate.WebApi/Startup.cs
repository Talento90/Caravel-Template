using System.Text.Json;
using System.Text.Json.Serialization;
using AutoMapper;
using FluentValidation.AspNetCore;
using Caravel.AppContext;
using Caravel.AspNetCore.Authentication;
using Caravel.AspNetCore.Middleware;
using Caravel.Clock;
using Caravel.Http;
using Caravel.MediatR.Behaviours;
using CaravelTemplate.Core.Books.Queries;
using CaravelTemplate.Core.Interfaces.Authentication;
using CaravelTemplate.Core.Interfaces.Data;
using CaravelTemplate.Core.Interfaces.Identity;
using CaravelTemplate.Infrastructure.Authentication;
using CaravelTemplate.Infrastructure.Data;
using CaravelTemplate.Infrastructure.Identity;
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
            services.ConfigureAuthentication(Configuration);
            services.ConfigureSwagger();
            services.AddRouting(r => r.LowercaseUrls = true);

            services.AddAutoMapper(typeof(GetBookByIdQuery).Assembly);

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPerformanceBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));
            services.AddAuthorizeFromAssembly(typeof(GetBookByIdQuery).Assembly);
            services.AddMediatR(typeof(GetBookByIdQuery).Assembly);

            services.AddHttpContextAccessor();
            services.AddScoped<IClock, DateTimeClock>();
            services.AddScoped<IAppContextAccessor, AppContextAccessor>();
            services.AddScoped<ITokenFactory, TokenFactory>();
            services.AddScoped<IJwtManager, JwtManager>();
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();

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

            app.UseMiddleware<TraceIdMiddleware>();
            app.UseMiddleware<AppContextEnricherMiddleware>();
            app.UseMiddleware<LoggingMiddleware>(Options.Create(new LoggingSettings()
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