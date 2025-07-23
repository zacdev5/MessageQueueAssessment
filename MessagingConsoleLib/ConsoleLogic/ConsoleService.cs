namespace MessagingConsoleLib.ConsoleLogic;

public class ConsoleService : IConsoleService
{
    // Prompt the user for their name with validation
    public Task<string> PromptForNameAsync()
    {
        Console.Write("Enter Name: ");
        string? input = Console.ReadLine();

        while (string.IsNullOrWhiteSpace(input))
        {
            Console.Write("Name cannot be empty. Please enter a name: ");
            input = Console.ReadLine();
        }

        return Task.FromResult(input);
    }

    // Print a message to the console (kind of redundant but gives more logic for injection)
    public void WriteMessage(string message)
    {
        Console.WriteLine(message);
    }
}
