
namespace MessagingConsoleLib.MessageLogic
{
    public interface IMessageService
    {
        ValueTask DisposeAsync();
        Task ReceiveMessageAsync(Func<string, Task> handleMessageAsync);
        Task SendMessageAsync(string msg);
        Task StartAsync(CancellationToken cancellationToken);
        Task StopAsync(CancellationToken cancellationToken);
    }
}