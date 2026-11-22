namespace backendProj.Shared.Messaging;

public sealed class RabbitMqOptions
{
    public string HostName { get; set; } =  "localhost";
    public int Port { get; set; } = 5672;
    public string UserName { get; set; } = "guest";
    public string Password { get; set; } = "guest";

    public string Exchange { get; set; } = "payments";
    public string Queue { get; set; } = "payments.pending";
    public string RoutingKey { get; set; } = "payment.created";
    public string RetryExchange { get; set; } = "payments.retry";
    public string RetryQueue { get; set; } = "payments.retry.wait";
    public int RetryDelayMs { get; set; } = 1000;
    public ushort PrefetchCount { get; set; } = 50;
    public ushort ConsumerConcurrency { get; set; } = 10;
}