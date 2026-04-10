using System.Text.Json;
using Application.Abstractions.Messaging;
using RabbitMQ.Client;

namespace Infrastructure.Messaging;

internal sealed class RabbitMQPublisher : IRabbitMQPublisher, IAsyncDisposable
{
    private const string ExchangeName = "document-intelligence";

    private readonly RabbitMqSettings _settings;
    private IConnection? _connection;

    public RabbitMQPublisher(RabbitMqSettings settings)
    {
        _settings = settings;
    }

    public async Task PublishAsync<T>(T message, string routingKey, CancellationToken cancellationToken)
        where T : class
    {
        IConnection connection = await GetConnectionAsync(cancellationToken);
        await using IChannel channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

        await channel.ExchangeDeclareAsync(
            exchange: ExchangeName,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false,
            cancellationToken: cancellationToken);

        byte[] body = JsonSerializer.SerializeToUtf8Bytes(message);

        await channel.BasicPublishAsync(
            exchange: ExchangeName,
            routingKey: routingKey,
            body: body,
            cancellationToken: cancellationToken);
    }

    private async Task<IConnection> GetConnectionAsync(CancellationToken cancellationToken)
    {
        if (_connection is { IsOpen: true })
        {
            return _connection;
        }

        ConnectionFactory factory = new()
        {
            HostName = _settings.Host,
            Port = _settings.Port,
            UserName = _settings.Username,
            Password = _settings.Password,
            VirtualHost = _settings.VirtualHost
        };

        _connection = await factory.CreateConnectionAsync(cancellationToken);
        return _connection;
    }

    public async ValueTask DisposeAsync()
    {
        if (_connection is not null)
        {
            await _connection.DisposeAsync();
        }
    }
}
