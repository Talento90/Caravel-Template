using System;
using System.Net;
using System.Text;
using Caravel.AspNetCore.Authentication;
using Caravel.AspNetCore.Http;
using Caravel.Errors;
using CaravelTemplate.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace CaravelTemplate.WebApi.Extensions
{
    public static class AuthenticationExtensions
    {
        public static void ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtIssuerSettings>(configuration.GetSection("Jwt"));

            var settings = configuration.GetSection("Jwt").Get<JwtIssuerSettings>();

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = settings.Issuer,
                ValidateAudience = true,
                ValidAudience = settings.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(settings.IssuerSigningKey)),
                RequireExpirationTime = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(configureOptions =>
                {
                    configureOptions.ClaimsIssuer = settings.Issuer;
                    configureOptions.TokenValidationParameters = tokenValidationParameters;
                    configureOptions.SaveToken = true;

                    configureOptions.Events = new JwtBearerEvents()
                    {
                        OnChallenge = async (context) =>
                        {
                            context.HandleResponse();

                            if (context.AuthenticateFailure != null)
                            {
                                context.Response.StatusCode = 401;
                                var httpError = new Error(
                                    context.Error ?? "invalid_token",
                                    context.ErrorDescription ?? "The access token is not valid."
                                );
                                await context.HttpContext.Response.WriteAsJsonAsync(
                                    new HttpError(
                                        context.HttpContext,
                                        HttpStatusCode.Unauthorized,
                                        httpError
                                    )
                                );
                            }
                        },
                        OnForbidden = async context =>
                        {
                            context.Response.StatusCode = 403;
                            
                            var httpError = new Error(
                                "invalid_permission",
                                "No permissions to access this resource."
                            );
                            await context.HttpContext.Response.WriteAsJsonAsync(
                                new HttpError(
                                    context.HttpContext,
                                    HttpStatusCode.Forbidden,
                                    httpError
                                )
                            );
                        }
                    };
                });

            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();

                options.AddPolicy("AdminOnly",
                    p => p.RequireRole(Roles.Admin));
            });
        }
    }
}