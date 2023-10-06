using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Bot250PingGod.Application;
using Infrastructure.DataAccess;
using Infrastructure.Seedwork.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace Bot250PingGod;

public static class Program
{
    public static async Task Main()
    {
        using CancellationTokenSource cts = new();

        var builder = new ConfigurationBuilder()
                      .AddJsonFile("appsettings.json")
                      .AddEnvironmentVariables();

        var configuration = builder.Build();

        ConnectionStringsManager.ReadFromConfiguration(configuration);

        var botToken = configuration.GetValue<string>("TelegramBotToken");

        var botClient = new TelegramBotClient(botToken!);

        var container = BuildContainer(botClient);

        var bot = container.Resolve<TelegramBot>();

        await bot.RunAsync(cts.Token);

        cts.Cancel();
    }

    private static IContainer BuildContainer(ITelegramBotClient botClient)
    {
        var services = new ServiceCollection();
        services.AddLogging(builder => //
        {
            builder.AddConsole();
        });

        var builder = new ContainerBuilder();
        builder.Populate(services);

        builder.RegisterAssemblyModules(Assembly.Load("Bot250PingGod.Application"));

        builder.RegisterInstance(botClient).As<ITelegramBotClient>();
        builder.RegisterInstance(NhSessionFactory.Instance);

        builder.RegisterInstance(new SystemEnvironmentConfiguration
        {
            SystemTimeZone = "Central Asia Standard Time",
            Standby        = false
        });

        return builder.Build();
    }
}