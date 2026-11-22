using System.Text.Json;
using backendProj.Services;
using backendProj.Shared.Messaging;
using backendProj.Shared.Models;
using RabbitMQ.Client;

namespace backendProj.Messaging;

public class RabbitMqPaymentQueue : IPaymentQueue, IAsyncDisposable
{
    private readonly IChannel _channel;
    private readonly RabbitMqOptions _options;
    private readonly SemaphoreSlim _publishLock = new(1, 1);
    private readonly BasicProperties _properties = new();

    private RabbitMqPaymentQueue(IChannel channel, RabbitMqOptions options)
    {
        _channel = channel;
        _options = options;
    }

    public static async Task<RabbitMqPaymentQueue> CreateAsync(IConnection connection, RabbitMqOptions options)
    {
        var channel = await connection.CreateChannelAsync(new CreateChannelOptions(
            publisherConfirmationsEnabled: true,
            publisherConfirmationTrackingEnabled: true));

        return new RabbitMqPaymentQueue(channel, options);
    }
    
    public async Task EnqueueAsync(Payment payment, CancellationToken cancellationToken)
    {
        var body = JsonSerializer.SerializeToUtf8Bytes(payment, JsonSerializerOptions.Web);
        
        await _publishLock.WaitAsync(cancellationToken);
        try
        {
            await _channel.BasicPublishAsync(
                exchange: _options.Exchange,
                routingKey: _options.RoutingKey,
                mandatory: true,
                basicProperties: _properties,
                body: body,
                cancellationToken: cancellationToken);
        }
        finally
        {
            _publishLock.Release();
        }
    }

    public async ValueTask DisposeAsync()
    {
        await _channel.CloseAsync();
        await _channel.DisposeAsync();
        _publishLock.Dispose();
    }
}