using MessagingConsoleLib.ConsoleLogic;
using MessagingConsoleLib.MessageLogic;
using Moq;

namespace MessagingConsoleTests.AppLogic;

public class AppTests
{
    [Fact]
    public async Task Run_SendsFormattedMessage()
    {
        // Create mock services
        var mockConsoleService = new Mock<IConsoleService>();
        var mockMessageService = new Mock<IMessageService>();

        var app = new SenderApp.App(mockConsoleService.Object, mockMessageService.Object);

        // Simulate entering "Luke"
        mockConsoleService
            .Setup(c => c.PromptForNameAsync())
            .ReturnsAsync("Luke");

        // Execute
        await app.Run(Array.Empty<string>());

        // Assert
        mockMessageService.Verify(m => m.SendMessageAsync("Hello my name is, Luke"), Times.Once());
    }

    [Fact]
    public async Task ReceiveMessageAsync_RespondsWithExpectedMessage()
    {
        // Create mock services
        var mockConsoleService = new Mock<IConsoleService>();
        var mockMessageService = new Mock<IMessageService>();

        var app = new ReceiverApp.App(mockConsoleService.Object, mockMessageService.Object);

        Func<string, Task>? capturedHandler = null;

        // Capture the callback passed to ReceiveMessageAsync
        mockMessageService
            .Setup(m => m.ReceiveMessageAsync(It.IsAny<Func<string, Task>>()))
            .Callback<Func<string, Task>>(handler => capturedHandler = handler)
            .Returns(Task.CompletedTask);

        // Execute
        await app.RunMessageHandler();

        // Simulate message received from RabbitMQ
        if (capturedHandler != null)
        {
            await capturedHandler("Hello my name is, Luke");
        }

        // Assert
        mockConsoleService.Verify(c => c.WriteMessage("Hello Luke, I am your father!"), Times.Once());
    }
}
