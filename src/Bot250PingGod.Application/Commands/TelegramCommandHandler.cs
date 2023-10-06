using Autofac;
using Infrastructure.DataAccess;
using Microsoft.Extensions.Logging;
using NHibernate;
using Telegram.Bot;

namespace Bot250PingGod.Application.Commands;

public sealed class TelegramCommandHandler
{
    private readonly ILogger<TelegramCommandHandler> _logger;
    private readonly ILifetimeScope _lifetimeScope;
    private readonly TelegramBotClient _telegramBotClient;

    public TelegramCommandHandler(ILogger<TelegramCommandHandler> logger,
                                  ILifetimeScope lifetimeScope,
                                  TelegramBotClient telegramBotClient)
    {
        _logger            = logger;
        _lifetimeScope     = lifetimeScope;
        _telegramBotClient = telegramBotClient;
    }

    public async Task HandleAsync(TelegramCommand command, CancellationToken cancellationToken)
    {
        var sessionFactory = _lifetimeScope.Resolve<ISessionFactory>();

        using (_logger.BeginScope("Command#{TelegramCommand}", command.Command))
            try
            {
                await using (DbSession.Bind(sessionFactory))
                {
                    _logger.LogInformation("Начало обработки команды {TelegramCommand}",
                                           command.Command);

                    if (command.IsEasterEgg)
                    {
                        var easterEggHandler = _lifetimeScope.Resolve<EasterEggHandler>();

                        await easterEggHandler.HandleAsync(command, cancellationToken);

                        return;
                    }

                    if (!_lifetimeScope.TryResolveKeyed<ITelegramCommandHandler>(command.Command, out var handler))
                    {
                        _logger.LogWarning("Невозможно обработать команду {TelegramCommand}, не найден обработчик",
                                           command.Command);
                        return;
                    }

                    await handler.HandleAsync(command, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Произошла ошибка при обработке команды {TelegramCommand}",
                                 command.Command);

                try
                {
                    await _telegramBotClient.SendTextMessageAsync(chatId: command.ChatId,
                                                                  text: "Внутренняя ошибка системы",
                                                                  cancellationToken: cancellationToken);
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "Произошла ошибка при попытке отправить сообщение о внутренней ошибке системы");
                }
            }
    }
}