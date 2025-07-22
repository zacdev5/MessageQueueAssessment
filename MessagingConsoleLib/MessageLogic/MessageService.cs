using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace MessagingConsoleLib.MessageLogic;

public class MessageService : IHostedService, IAsyncDisposable, IMessageService
{
    private IConnection? _connection;
    private IChannel? _channel;
    private readonly string _exchangeName = "DemoExchange";
    private readonly string _routingKey = "demo-routing-key";
    private readonly string _queueName = "DemoQueue";
    private readonly string _rabbitMQConn = "amqp://guest:guest@localhost:5672";

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var factory = new ConnectionFactory();
        factory.Uri = new Uri(_rabbitMQConn);

        _connection = await factory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();

        await _channel.ExchangeDeclareAsync(_exchangeName, ExchangeType.Direct);
        await _channel.QueueDeclareAsync(_queueName, false, false, false, null);
        await _channel.QueueBindAsync(_queueName, _exchangeName, _routingKey, null);
    }

    public async Task SendMessageAsync(string msg)
    {
        if (_channel == null)
        {
            throw new InvalidOperationException("Channel is not initialized.");
        }

        byte[] msgBodyBytes = Encoding.UTF8.GetBytes(msg);
        var props = new BasicProperties();
        await _channel.BasicPublishAsync(_exchangeName, _routingKey, false, props, msgBodyBytes);
    }

    public async Task ReceiveMessageAsync(Func<string, Task> handleMessageAsync)
    {
        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (ch, ea) =>
        {
            var body = ea.Body.ToArray();

            string received = Encoding.UTF8.GetString(body);

            // Call your custom logic with message
            await handleMessageAsync(received);

            await _channel.BasicAckAsync(ea.DeliveryTag, false);
        };

        string consumerTag = await _channel.BasicConsumeAsync(_queueName, false, consumer);

        await _channel.BasicCancelAsync(consumerTag);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel?.IsOpen == true)
        {
            await _channel.CloseAsync();
            await _channel.DisposeAsync();
        }

        if (_connection?.IsOpen == true)
        {
            await _connection.CloseAsync();
            await _connection.DisposeAsync();
        }
    }
}
