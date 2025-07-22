using RabbitMQ.Client;
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

Console.Write("Enter Name: ");
string? nameInput = Console.ReadLine();

byte[] messageBodyBytes = Encoding.UTF8.GetBytes($"Hello my name is, {nameInput}");
var props = new BasicProperties();
await channel.BasicPublishAsync(exchangeName, routingKey, false, props, messageBodyBytes);

await channel.CloseAsync();
await conn.CloseAsync();

await channel.DisposeAsync();
await conn.DisposeAsync();