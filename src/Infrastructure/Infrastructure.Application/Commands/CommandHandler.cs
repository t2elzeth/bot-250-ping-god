using Autofac;

namespace Infrastructure.Application.Commands;

public sealed class CommandHandler
{
    private readonly Dictionary<Type, object> _cache = new();

    private readonly ILifetimeScope _serviceFactory;

    public CommandHandler(ILifetimeScope serviceFactory)
    {
        _serviceFactory = serviceFactory;
    }

    public async Task<CommandHandlerResult> HandleAsync<TCommand>(TCommand command)
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

        return await HandleAsync(command, cts.Token);
    }

    public async Task<CommandHandlerResult> HandleAsync<TCommand>(TCommand command, CancellationToken cancellationToken)
    {
        ICommandHandler<TCommand> handler;
        lock (_cache)
        {
            if (!_cache.TryGetValue(typeof(ICommandHandler<TCommand>), out var temp))
            {
                var commandHandler = _serviceFactory.Resolve<ICommandHandler<TCommand>>();
                
                handler = CommandHandlerDecoratorHelper.Decorate(_serviceFactory, commandHandler.GetType(), commandHandler);

                _cache[typeof(ICommandHandler<TCommand>)] = handler;
            }
            else
            {
                handler = (ICommandHandler<TCommand>)temp;
            }
        }

        return await handler.HandleAsync(command, cancellationToken);
    }
}