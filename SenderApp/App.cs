using MessagingConsoleLib.ConsoleLogic;

namespace SenderApp;

public class App
{
    private readonly IConsoleService _console;

    public App(IConsoleService console)
    {
        _console = console;
    }

    public async Task Run(string[] args)
    {
        string name = await _console.PromptForNameAsync();
        _console.WriteMessage($"Hello {name}");
    }
}
