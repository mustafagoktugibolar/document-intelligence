namespace Application.Abstractions.Messaging;

public interface IRabbitMQPublisher
{
    Task PublishAsync<T>(T message, string routingKey, CancellationToken cancellationToken) where T : class;
}
