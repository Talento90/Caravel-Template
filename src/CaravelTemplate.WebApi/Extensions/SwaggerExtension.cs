using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CaravelTemplate.WebApi.Extensions
{
    public static class SwaggerExtension
    {
        public static void ConfigureSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo()
                {
                    Version = "v1",
                    Title = "CaravelTemplate API",
                    Description = "CaravelTemplate Description.",
                });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(System.AppContext.BaseDirectory, xmlFile);
                
                var bearerScheme = new OpenApiSecurityScheme()
                {
                    Name = "Bearer Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Description = "Bearer Token Authorization header. e.g. Authorization: Bearer {token}",
                    Scheme = "Bearer",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    },
                };

                c.AddSecurityDefinition("Bearer", bearerScheme);

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {bearerScheme, new List<string>()},
                });
                
                c.IncludeXmlComments(xmlPath);
                //c.AddFluentValidationRules();
            });
        }
    }
}