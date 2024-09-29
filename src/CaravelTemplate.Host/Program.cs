using CaravelTemplate.Adapter.Api;
using CaravelTemplate.Adapter.Identity;
using CaravelTemplate.Adapter.PostgreSql;
using CaravelTemplate.Adapter.PostgreSql.Repositories;
using CaravelTemplate.Application.Data;
using CaravelTemplate.Application.Metrics;
using CaravelTemplate.Host.Extensions;
using Serilog;

try
{
    var webApi = new ApiServer(args, (builder) =>
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .CreateLogger();
        
        builder.Logging.ClearProviders();
        builder.Services.AddSerilog();
        
        builder.Services.AddSingleton<BookMetrics>();
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        builder.Services.AddScoped<IBookRepository, BookRepository>();
        
        builder.Services.AddAdapterPostgreSql(builder.Configuration);
        builder.Services.AddIdentityAdapter(builder.Configuration);
        builder.Services.AddOpenTelemetry(builder.Configuration, builder.Environment);
        
        //builder.Services.AddQuartzAdapter(builder.Configuration);
        //builder.Services.AddMassTransitAdapter(builder.Configuration);
    }, (application =>
    {
        // Map other modules endpoints
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


// This dummy class is needed for integration tests WebApplicationFactory
public partial class Program { }