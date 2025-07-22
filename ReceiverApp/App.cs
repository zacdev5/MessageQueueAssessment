using MessagingConsoleLib.ConsoleLogic;
using MessagingConsoleLib.MessageLogic;

namespace ReceiverApp;

public class App
{
    private readonly IConsoleService _consoleService;
    private readonly IMessageService _messageService;

    public App(IConsoleService console, IMessageService messageService)
    {
        _consoleService = console;
        _messageService = messageService;
    }

    public async Task Run(string[] args)
    {
        await _messageService.ReceiveMessageAsync(async message =>
        {
            _consoleService.WriteMessage(message);
            await Task.CompletedTask;
        });

        _consoleService.WriteMessage("Receiver is running. Press Enter to exit...");
        Console.ReadLine();
    }
}
