using backendProj.Messaging;
using backendProj.Services;
using backendProj.Shared.Messaging;

var builder = WebApplication.CreateBuilder(args);

var rabbitOptions = builder.Configuration.GetSection("RabbitMQ").Get<RabbitMqOptions>() ?? new RabbitMqOptions();

var connection = await RabbitMqSetup.ConnectAsync(rabbitOptions);
await RabbitMqSetup.DeclareTopologyAsync(connection, rabbitOptions);
var paymentQueue = await RabbitMqPaymentQueue.CreateAsync(connection, rabbitOptions);

builder.Services.AddSingleton(rabbitOptions);
builder.Services.AddSingleton(connection);
builder.Services.AddSingleton<IPaymentQueue>(paymentQueue);
builder.Services.AddControllers();

var app = builder.Build();
app.MapControllers();

await app.RunAsync();

await paymentQueue.DisposeAsync();
await connection.DisposeAsync();
