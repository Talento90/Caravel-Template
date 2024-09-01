using CaravelTemplate.Adapter.Api;
using CaravelTemplate.Adapter.Identity;
using CaravelTemplate.Adapter.MassTransit;
using CaravelTemplate.Adapter.PostgreSql;
using CaravelTemplate.Adapter.PostgreSql.Repositories;
using CaravelTemplate.Adapter.Quartz;
using CaravelTemplate.Application.Data;
using CaravelTemplate.Application.Metrics;
using Serilog;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", true)
    .AddEnvironmentVariables()
    .Build();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();

try
{
    var webApi = new ApiServer(args, (builder) =>
    {
        // Logging
        builder.Logging.ClearProviders();
        builder.Services.AddSerilog();
        
        builder.Services.AddSingleton<BookMetrics>();
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        builder.Services.AddScoped<IBookRepository, BookRepository>();

        // Database
        var postgreOption = builder.Configuration
                                .GetSection("PostgreSql")
                                .Get<PostgreSqlOptions>() ?? throw new NullReferenceException(nameof(PostgreSqlOptions));
        builder.Services.RegisterPostgreSql(postgreOption);

        // Quartz
        var quartzOption = builder.Configuration
            .GetSection("Quartz")
            .Get<QuartzOptions>() ?? throw new NullReferenceException(nameof(QuartzOptions));
        builder.Services.RegisterQuartz(quartzOption);

        // Identity
        var identityOption = builder.Configuration
            .GetSection("Identity")
            .Get<IdentityOptions>() ?? throw new NullReferenceException(nameof(IdentityOptions));
        builder.Services.RegisterIdentity(identityOption);

        // MassTransit
        var massTransitOption = builder.Configuration
            .GetSection("MassTransit")
            .Get<MassTransitOptions>() ?? throw new NullReferenceException(nameof(MassTransitOptions));
        builder.Services.RegisterMassTransit(massTransitOption);
    }, (application =>
    {
        // Map Custom Endpoints
        application.MapIdentityEndpoints();
    }));

    Log.Information("Starting CaravelTemplate.Host");

    await webApi.StartAsync();
}
catch (Exception e)
{
    Log.Error(e, "Failed to start CaravelTemplate.Host");
}
finally
{
    await Log.CloseAndFlushAsync();
}