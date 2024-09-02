using CaravelTemplate.Adapter.Identity;
using CaravelTemplate.Adapter.MassTransit;
using CaravelTemplate.Adapter.PostgreSql;
using CaravelTemplate.Adapter.Quartz;

namespace CaravelTemplate.Host.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddAdapterPostgreSql(
        this IServiceCollection services,
        IConfigurationManager configuration)
    {
        var postgreOption = configuration
            .GetSection("PostgreSql")
            .Get<PostgreSqlOptions>() ?? throw new NullReferenceException(nameof(PostgreSqlOptions));
        services.RegisterPostgreSql(postgreOption);
    }

    public static void AddQuartzAdapter(
        this IServiceCollection services,
        IConfigurationManager configuration)
    {
        var quartzOption = configuration
            .GetSection("Quartz")
            .Get<QuartzOptions>() ?? throw new NullReferenceException(nameof(QuartzOptions));
        services.RegisterQuartz(quartzOption);
    }

    public static void AddIdentityAdapter(
        this IServiceCollection services,
        IConfigurationManager configuration)
    {
        var identityOption = configuration
            .GetSection("Identity")
            .Get<IdentityOptions>() ?? throw new NullReferenceException(nameof(IdentityOptions));
        services.RegisterIdentity(identityOption);
    }
    
    public static void AddMassTransitAdapter(
        this IServiceCollection services,
        IConfigurationManager configuration)
    {
        var massTransitOption = configuration
            .GetSection("MassTransit")
            .Get<MassTransitOptions>() ?? throw new NullReferenceException(nameof(MassTransitOptions));
        services.RegisterMassTransit(massTransitOption);
    }
}