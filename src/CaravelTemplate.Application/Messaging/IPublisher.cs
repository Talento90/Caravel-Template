namespace CaravelTemplate.Application.Messaging;

public interface IPublisher
{
    Task PublishAsync<T>(T message, CancellationToken ct) where T : IMessage;
}