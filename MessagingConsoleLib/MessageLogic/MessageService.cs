using MessagingConsoleLib.Configurations;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace MessagingConsoleLib.MessageLogic;

public class MessageService : IHostedService, IAsyncDisposable, IMessageService
{
    private readonly RabbitMQSettings _settings;
    private IConnection? _connection;
    private IChannel? _channel;

    public MessageService(IOptions<RabbitMQSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var factory = new ConnectionFactory();
        factory.Uri = new Uri(_settings.Uri);

        try
        {
            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();
        }
        catch (Exception ex)
        {
            throw new ApplicationException("RabbitMQ broker is unreachable.", ex);
        }

        await _channel.ExchangeDeclareAsync(_settings.ExchangeName, ExchangeType.Direct);
        await _channel.QueueDeclareAsync(_settings.QueueName, false, false, false, null);
        await _channel.QueueBindAsync(_settings.QueueName, _settings.ExchangeName, _settings.RoutingKey, null);
    }

    public async Task SendMessageAsync(string msg)
    {
        if (_channel == null)
        {
            throw new InvalidOperationException("Channel is not initialized.");
        }

        byte[] msgBodyBytes = Encoding.UTF8.GetBytes(msg);
        var props = new BasicProperties();
        await _channel.BasicPublishAsync(_settings.ExchangeName, _settings.RoutingKey, false, props, msgBodyBytes);
    }

    public async Task ReceiveMessageAsync(Func<string, Task> handleMessageAsync)
    {
        if (_channel == null)
        {
            throw new InvalidOperationException("Channel is not initialized.");
        }

        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (ch, ea) =>
        {
            var body = ea.Body.ToArray();

            string received = Encoding.UTF8.GetString(body);

            // Call your custom logic with message
            await handleMessageAsync(received);

            await _channel.BasicAckAsync(ea.DeliveryTag, false);
        };

        string consumerTag = await _channel.BasicConsumeAsync(_settings.QueueName, false, consumer);

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
