using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text.Json;
using System.Text;

namespace Charlie.Payment.RMQ;

public class RabbitMqClient
{
    private IConnection _connection;
    private IChannel _channel;

    public RabbitMqClient(IConfiguration configuration)
    {
        InitializeAsync(configuration).GetAwaiter().GetResult();
    }

    public async Task InitializeAsync(IConfiguration configuration)
    {
        var factory = new ConnectionFactory
        {
            HostName = configuration["RabbitMq:HostName"],
            UserName = configuration["RabbitMq:UserName"],
            Password = configuration["RabbitMq:Password"]
        };
        _connection = await factory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();

        AppDomain.CurrentDomain.ProcessExit += (s, e) => Dispose();

    }

    public async Task PublishAsync(string queueName, object message)
    {
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

        var properties = new BasicProperties();
        properties.Persistent = true;
        await _channel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false);
        await _channel.BasicPublishAsync(exchange: "", routingKey: queueName, mandatory: true, basicProperties: properties, body: body);

        await Task.CompletedTask;
    }

    public async Task SubscribeAsync(string queueName, Func<string, Task> onMessageReceived, CancellationToken cancellationToken)
    {
        _channel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            await onMessageReceived(message);
        };

        _channel.BasicConsumeAsync(queue: queueName, autoAck: true, consumer: consumer);

        await Task.CompletedTask;
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
}
