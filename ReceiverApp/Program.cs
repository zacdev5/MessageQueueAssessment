using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

ConnectionFactory factory = new ConnectionFactory();

factory.Uri = new Uri("amqp://guest:guest@localhost:5672");

IConnection conn = await factory.CreateConnectionAsync();
IChannel channel = await conn.CreateChannelAsync();

string exchangeName = "DemoExchange";
string routingKey = "demo-routing-key";
string queueName = "DemoQueue";

await channel.ExchangeDeclareAsync(exchangeName, ExchangeType.Direct);
await channel.QueueDeclareAsync(queueName, false, false, false, null);
await channel.QueueBindAsync(queueName, exchangeName, routingKey, null);


var consumer = new AsyncEventingBasicConsumer(channel);
consumer.ReceivedAsync += async (ch, ea) =>
{
    byte[] body = ea.Body.ToArray();

    string receivedMessage = Encoding.UTF8.GetString(body);
    Console.WriteLine(receivedMessage);

    await channel.BasicAckAsync(ea.DeliveryTag, false);
};
string consumerTag = await channel.BasicConsumeAsync(queueName, false, consumer);

Console.ReadLine();

await channel.BasicCancelAsync(consumerTag);

await channel.CloseAsync();
await conn.CloseAsync();

await channel.DisposeAsync();
await conn.DisposeAsync();