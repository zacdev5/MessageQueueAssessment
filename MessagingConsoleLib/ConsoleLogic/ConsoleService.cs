namespace MessagingConsoleLib.ConsoleLogic;

public class ConsoleService : IConsoleService
{
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

    public void WriteMessage(string message)
    {
        Console.WriteLine(message);
    }
}
