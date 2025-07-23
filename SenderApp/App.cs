using MessagingConsoleLib.ConsoleLogic;
using MessagingConsoleLib.MessageLogic;

namespace SenderApp;

public class App
{
    private readonly IConsoleService _consoleService;
    private readonly IMessageService _messageService;

    // Constructor with dependency injection
    public App(IConsoleService console, IMessageService messageService)
    {
        _consoleService = console;
        _messageService = messageService;
    }

    // Runs the main logic: gets user input and sends a message
    public async Task Run(string[] args)
    {
        string name = await _consoleService.PromptForNameAsync();
        string message = $"Hello my name is, {name}";

        await _messageService.SendMessageAsync(message);
    }
}