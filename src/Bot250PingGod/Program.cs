using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Bot250PingGod.Application;
using Infrastructure.DataAccess;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace Bot250PingGod;

public static class Program
{
    private const string TelegramBotToken = "6678696659:AAGzrzRk9ajwD83QBrcrdAL0GBZkPmtR9Eg";

    public static async Task Main()
    {
        using CancellationTokenSource cts = new();

        var builder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json");

        var configuration = builder.Build();

        ConnectionStringsManager.ReadFromConfiguration(configuration);

        var botClient = new TelegramBotClient(TelegramBotToken);

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

        return builder.Build();
    }
}