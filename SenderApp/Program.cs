using MessagingConsoleLib.ConsoleLogic;
using MessagingConsoleLib.MessageLogic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SenderApp;

using IHost host = CreateHostBuilder(args).Build();
await host.StartAsync();

// Prevents app from 'violently' crashing
try
{
    using var scope = host.Services.CreateScope();
    var services = scope.ServiceProvider;

    await services.GetRequiredService<App>().Run(args);
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}

static IHostBuilder CreateHostBuilder(string[] args)
{
    return Host.CreateDefaultBuilder(args)
        .ConfigureServices((_, services) =>
        {
            services.AddSingleton<IConsoleService, ConsoleService>();
            services.AddSingleton<App>();
            services.AddSingleton<MessageService>(); // Register concrete first
            services.AddSingleton<IMessageService>(sp => sp.GetRequiredService<MessageService>()); // Map interfaces to same instance
            services.AddHostedService(sp => sp.GetRequiredService<MessageService>()); // Register as hosted service
        });
}