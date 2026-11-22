using System.Diagnostics;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace backendProj.Shared.Messaging;

public static class RabbitMqSetup
{
    public static async Task<IConnection> ConnectAsync(RabbitMqOptions options, CancellationToken ct = default)
    {
        var factory = new ConnectionFactory
        {
            HostName = options.HostName,
            Port = options.Port,
            UserName = options.UserName,
            Password = options.Password,
            ClientProvidedName = "rinha-backend",
        };
        const int maxAttempts = 15;
        
        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                return await factory.CreateConnectionAsync(ct);
            }
            catch (BrokerUnreachableException) when (attempt < maxAttempts)
            {
                Console.WriteLine($"[rabbitmq] indisponível (tentativa {attempt})");
                await Task.Delay(TimeSpan.FromSeconds(2), ct);
            }
        }

        // Na última tentativa o filtro "when" é falso e a exceção sobe, então esta linha nunca executa.
        // Ela só existe porque o compilador não consegue provar isso sozinho (CS0161).
        throw new UnreachableException();
    }

    public static async Task DeclareTopologyAsync(IConnection connection, RabbitMqOptions options)
    {
        await using var channel = await connection.CreateChannelAsync();

        await channel.ExchangeDeclareAsync(options.Exchange, ExchangeType.Direct, durable: true);
        await channel.QueueDeclareAsync(options.Queue, durable: true, exclusive: false, autoDelete: false,
            arguments: new Dictionary<string, object?>
            {
                ["x-dead-letter-exchange"] = options.RetryExchange,
            });
        await channel.QueueBindAsync(options.Queue, options.Exchange, options.RoutingKey);

        await channel.ExchangeDeclareAsync(options.RetryExchange, ExchangeType.Direct, durable: true);
        await channel.QueueDeclareAsync(options.RetryQueue, durable: true, exclusive: false, autoDelete: false,
            arguments: new Dictionary<string, object?>
            {
                ["x-message-ttl"] = options.RetryDelayMs,
                ["x-dead-letter-exchange"] = options.Exchange,
            });
        await channel.QueueBindAsync(options.RetryQueue, options.RetryExchange, options.RoutingKey);
    }
}