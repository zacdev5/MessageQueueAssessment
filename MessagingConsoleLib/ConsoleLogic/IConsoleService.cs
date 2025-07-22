
namespace MessagingConsoleLib.ConsoleLogic
{
    public interface IConsoleService
    {
        Task<string> PromptForNameAsync();
        void WriteMessage(string message);
    }
}