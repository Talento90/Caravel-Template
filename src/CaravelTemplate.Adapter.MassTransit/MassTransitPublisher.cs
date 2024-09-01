using CaravelTemplate.Application.Messaging;
using CaravelTemplate.Books;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace CaravelTemplate.Adapter.MassTransit;

public class MassTransitPublisher(IBus bus, ILogger<MassTransitPublisher> logger) : IPublisher
{
    private readonly IBus _bus = bus;
    private readonly ILogger<MassTransitPublisher> _logger = logger;

    public async Task PublishAsync<T>(T message, CancellationToken ct) where T : IMessage 
    {
        _logger.LogInformation("Publish message {Id}", message.Id);
        await _bus.Publish(message ?? throw new ArgumentNullException(nameof(message)), ct);
    }
}