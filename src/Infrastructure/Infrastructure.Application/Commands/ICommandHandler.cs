using Infrastructure.Seedwork.Validation;

namespace Infrastructure.Application.Commands;

public interface ICommandHandler<in TCommand>
{
    Task<CommandHandlerResult> HandleAsync(TCommand command, CancellationToken cancellationToken);
}

[ApplicationCommandHandlerDecorator]
public abstract class BaseCommandAsyncHandler<TCommand> : ICommandHandler<TCommand>
{
    public abstract Task<CommandHandlerResult> HandleAsync(TCommand command, CancellationToken cancellationToken);

    protected static CommandHandlerResult Success()
    {
        return CommandHandlerResult.Unit;
    }

    protected static CommandHandlerResult Success(object value)
    {
        return CommandHandlerResult.Success(value);
    }

    protected static CommandHandlerResult Error(SystemError error)
    {
        return CommandHandlerResult.Failure(error);
    }
}
