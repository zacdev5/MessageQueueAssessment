using MessagingConsoleLib.ConsoleLogic;
using MessagingConsoleLib.MessageLogic;

namespace SenderApp;

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
        string name = await _consoleService.PromptForNameAsync();
        await _messageService.SendMessageAsync(name);
        //_console.WriteMessage($"Hello {name}");
    }
}