namespace MessagingConsoleLib.Configurations;

public class RabbitMQSettings
{
    public string Uri { get; set; } = "";
    public string ExchangeName { get; set; } = "";
    public string RoutingKey { get; set; } = "";
    public string QueueName { get; set; } = "";
}
