using Autofac;
using Infrastructure.DataAccess;
using Microsoft.Extensions.Logging;
using NHibernate;

namespace Bot250PingGod.Application.Commands;

public sealed class TelegramCommandHandler
{
    private readonly ILogger<TelegramCommandHandler> _logger;
    private readonly ILifetimeScope _lifetimeScope;

    public TelegramCommandHandler(ILogger<TelegramCommandHandler> logger,
                                  ILifetimeScope lifetimeScope)
    {
        _logger        = logger;
        _lifetimeScope = lifetimeScope;
    }

    public async Task HandleAsync(TelegramCommand command, CancellationToken cancellationToken)
    {
        var sessionFactory = _lifetimeScope.Resolve<ISessionFactory>();

        using (_logger.BeginScope("Command#{TelegramCommand}", command.Command))
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
}