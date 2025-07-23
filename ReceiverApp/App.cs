using MessagingConsoleLib.ConsoleLogic;
using MessagingConsoleLib.MessageLogic;

namespace ReceiverApp;

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

    // Runs the main logic: consume message and log output
    public async Task Run(string[] args)
    {
        // Listens for messages
        await RunMessageHandler();

        // Inform user that the receiver is running
        _consoleService.WriteMessage("Receiver is running. Press Enter to exit...");

        // Keep the console window open until Enter is pressed
        Console.ReadLine();
    }

    public async Task RunMessageHandler()
    {
        // Start listening for messages from RabbitMQ
        await _messageService.ReceiveMessageAsync(async message =>
        {
            // Extract name from message (assuming format is: "Hello my name is, {name}")
            string name = message.Substring(18);

            // Create and print response message to console
            string response = $"Hello {name}, I am your father!";
            _consoleService.WriteMessage(response);

            await Task.CompletedTask;
        });
    }
}
