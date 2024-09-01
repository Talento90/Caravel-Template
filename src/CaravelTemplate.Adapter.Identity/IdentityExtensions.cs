using Asp.Versioning;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CaravelTemplate.Adapter.Identity;

public static class IdentityExtensions
{
    public static void RegisterIdentity(this IServiceCollection services, IdentityOptions options)
    {
        services.AddIdentityCore<User>((identityOptions =>
            {
                identityOptions.Password.RequireDigit = true;
                identityOptions.User.RequireUniqueEmail = true;
            }))
            .AddEntityFrameworkStores<IdentityDbContext>()
            .AddApiEndpoints();
        
        services.AddDbContext<IdentityDbContext>(dbBuilder =>
        {
            dbBuilder.UseNpgsql(options.ConnectionString);
        });
    }
    
    public static void MapIdentityEndpoints(this WebApplication application)
    {
        // API Versioning
        var apiVersionSet = application.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1))
            .ReportApiVersions()
            .Build();
        
        application
            .MapGroup("/api/v{version:apiVersion}/identity")
            .WithTags("Identity")
            .MapCustomIdentityApi<User>()
            .WithApiVersionSet(apiVersionSet);
    }
}