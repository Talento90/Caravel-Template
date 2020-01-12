using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace CaravelTemplate.WebApi.Extensions
{
    public static class AuthenticationExtensions
    {
        public static void ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = configuration["Cognito:Authority"];
                    options.IncludeErrorDetails = true;

                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        RequireExpirationTime = true,
                        RequireSignedTokens = true,
                        ValidateActor = true,
                        ValidateAudience = false,
                        ValidateIssuer = true,
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,
                        ValidateTokenReplay = true,
                        ValidIssuer = options.Authority,
                    };
                });
        }
    }
}