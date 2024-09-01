using System.Text.Json;
using CaravelTemplate.Adapter.Quartz.Jobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.AspNetCore;

namespace CaravelTemplate.Adapter.Quartz;

public static class CollectionExtension
{
    public static void RegisterQuartz(this IServiceCollection services, QuartzOptions options)
    {
        services.AddDbContext<QuartzDbContext>((optionsBuilder) =>
        {
            optionsBuilder.UseNpgsql(options.ConnectionString);
        });
        
        services.AddQuartz(quartzConfig =>
        {
            quartzConfig.SchedulerName = options.SchedulerName;

            quartzConfig.UsePersistentStore(storeOptions =>
            {
                storeOptions.UseSystemTextJsonSerializer();
                storeOptions.UsePostgres((config) =>
                {
                    config.ConnectionString = options.ConnectionString;
                    config.TablePrefix = "quartz.qrtz_";
                });
            });
            
            quartzConfig.UseDefaultThreadPool(tp => { tp.MaxConcurrency = 10; });

            // Schedule jobs
            quartzConfig.ScheduleJob<HelloWorldJob>(trigger => trigger
                .WithIdentity(nameof(HelloWorldJob))
                .StartNow()
                .WithSimpleSchedule(x =>
                    x.WithInterval(TimeSpan.FromSeconds(60))
                        .RepeatForever()
                )
                .WithDescription("Hello World Job triggers every 60 seconds forever.")
            );
        });

        services.AddQuartzServer(server =>
        {
            server.AwaitApplicationStarted = true;
            // when ASP.NET is shutting down we want jobs to complete gracefully
            server.WaitForJobsToComplete = true;
        });
    }
}