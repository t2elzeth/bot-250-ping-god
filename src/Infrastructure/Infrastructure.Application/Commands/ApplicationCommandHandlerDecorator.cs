using System.Diagnostics;
using Infrastructure.Application.DomainEvents;
using Infrastructure.DataAccess;
using Infrastructure.Seedwork.DomainEvents;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Application.Commands;

public sealed class ApplicationCommandHandlerDecoratorAttribute : CommandHandlerDecoratorAttribute
{
    public ApplicationCommandHandlerDecoratorAttribute()
        : base(typeof(ApplicationCommandHandlerDecorator<>))
    {
    }
}

internal sealed class ApplicationCommandHandlerDecorator<TCommand> : ICommandHandler<TCommand>
{
    private readonly ICommandHandler<TCommand> _next;
    private readonly ILogger<ApplicationCommandHandlerDecorator<TCommand>> _logger;
    private readonly DomainEventsHandler _domainEventsHandler;

    public ApplicationCommandHandlerDecorator(ICommandHandler<TCommand> next,
                                              ILogger<ApplicationCommandHandlerDecorator<TCommand>> logger,
                                              DomainEventsHandler domainEventsHandler)
    {
        _next                = next;
        _logger              = logger;
        _domainEventsHandler = domainEventsHandler;
    }

    public async Task<CommandHandlerResult> HandleAsync(TCommand command, CancellationToken cancellationToken)
    {
        var runId = Guid.NewGuid().ToString("N");

        using (_logger.BeginScope("RunId#{RunId}", runId))
        {
            _logger.LogInformation("Обработка команды {Command}", typeof(TCommand).Name);

            var stopwatch = Stopwatch.StartNew();

            using var domainEventsSession = DomainEventsSession.Bind();

            await using var dbTransaction = new DbTransaction();

            var result = await _next.HandleAsync(command, cancellationToken);

            if (result.IsFailure)
            {
                await dbTransaction.RollbackAsync();

                _logger.LogInformation("Обработка команды {Command} завершилась с ошибкой: {@Error}",
                                       typeof(TCommand).Name, result.Error);
            }
            else
            {
                await _domainEventsHandler.HandleAsync(domainEventsSession, cancellationToken);

                await dbTransaction.CommitAsync();

                _logger.LogInformation("Обработка команды {Command}#{@CommandArgs} завершилась успешно. Результат выполнения: {@Result}",
                                       typeof(TCommand).Name, command, result.Value);
            }

            stopwatch.Stop();

            _logger.LogInformation("Обработка команды {Command} заняла {Time}", typeof(TCommand).Name, stopwatch.Elapsed);

            return result;
        }
    }
}