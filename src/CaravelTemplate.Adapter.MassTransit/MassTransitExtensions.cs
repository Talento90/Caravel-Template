using CaravelTemplate.Application.Messaging;
using MassTransit;
using MassTransit.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CaravelTemplate.Adapter.MassTransit;

public static class MassTransitExtensions
{
    public static void RegisterMassTransit(this IServiceCollection services, MassTransitOptions options)
    {
        services.AddMassTransit((busConfig) =>
        {
            // Endpoints where we publish messages
            busConfig.SetKebabCaseEndpointNameFormatter();
            
            // Register all consumers via assembly scan
            busConfig.AddConsumers(typeof(IMassTransitAssemblyMarker).Assembly);

            //  Configure which messaging transport (RabbitMq, SQS, etc)
             // busConfig.UsingInMemory((context, config) =>
             // {
             //     config.ConfigureEndpoints(context);
             // });
             
             busConfig.UsingRabbitMq((context, config) =>
             {
                 config.Host(options.Host, options.Port, options.VirtualHost, host =>
                 {
                     host.Username(options.Username);
                     host.Password(options.Password);
                 });
                 
                 config.ConfigureEndpoints(context);
             });
            
            services.AddSingleton<IPublisher, MassTransitPublisher>();
        });
    }
}