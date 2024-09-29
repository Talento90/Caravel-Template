using CaravelTemplate.Application.Messaging;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace CaravelTemplate.Adapter.MassTransit.Consumers;

public class BookCreatedConsumer : IConsumer<BookCreatedMessage>
{
    private readonly ILogger<BookCreatedConsumer> _logger;

    public BookCreatedConsumer(ILogger<BookCreatedConsumer> logger)
    {
        _logger = logger;
    }


    public Task Consume(ConsumeContext<BookCreatedMessage> context)
    {
        _logger.LogInformation("Consuming {Id} {Name}", context.Message.Book.Id, context.Message.Book.Name);

        return Task.CompletedTask;
    }
}