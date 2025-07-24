using MessagingConsoleLib.Configurations;
using MessagingConsoleLib.ConsoleLogic;
using MessagingConsoleLib.MessageLogic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SenderApp;

// Build and start host
using IHost host = CreateHostBuilder(args).Build();
await host.StartAsync();

// Prevents app from 'violently' crashing
try
{
    using var scope = host.Services.CreateScope();
    var services = scope.ServiceProvider;

    // Run main application
    await services.GetRequiredService<App>().Run(args);
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}

// Configure host, services and configuration
static IHostBuilder CreateHostBuilder(string[] args)
{
    return Host.CreateDefaultBuilder(args)
        .ConfigureLogging(logging =>
        {
            logging.AddConsole();
            logging.SetMinimumLevel(LogLevel.Warning); // To show clean console interface
        })
        .ConfigureAppConfiguration((context, config) =>
        {
            // Load settings from appsettings.json
            config.AddJsonFile("appsettings.json", false, true);

            //Enable environment variables override
            config.AddEnvironmentVariables();
        })
        .ConfigureServices((_, services) =>
        {
            // Bind appsettings.json to RabbitMQSettings class
            services.Configure<RabbitMQSettings>(_.Configuration.GetSection("RabbitMQ"));

            // Inject ConsoleService as singleton
            services.AddSingleton<IConsoleService, ConsoleService>();

            //Inject App as singleton
            services.AddSingleton<App>();

            // Inject MessageService as singleton and to register concrete first (work around because of ambiguity in IHostedService)
            services.AddSingleton<MessageService>();
            services.AddSingleton<IMessageService>(sp => sp.GetRequiredService<MessageService>()); // Map interfaces to same instance

            // Register as hosted service
            services.AddHostedService(sp => sp.GetRequiredService<MessageService>());
        });
}