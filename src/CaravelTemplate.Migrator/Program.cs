using CaravelTemplate.Migrator.Extensions;
using Microsoft.Extensions.Hosting;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();
try
{
    Log.Information("Starting up Migrator");

    builder.Services.AddApplicationDbContext(builder.Configuration);
    builder.Services.AddQuartzDbContext(builder.Configuration);
    builder.Services.AddIdentityDbContext(builder.Configuration);

    builder.Build();

    Log.Information("Migrator Run Successfully");
}
catch (Exception ex) when
    (ex is not HostAbortedException &&
     ex.Source != "Microsoft.EntityFrameworkCore.Design") // see https://github.com/dotnet/efcore/issues/29923
{
    Log.Error(ex, "An error occured during migration");
}