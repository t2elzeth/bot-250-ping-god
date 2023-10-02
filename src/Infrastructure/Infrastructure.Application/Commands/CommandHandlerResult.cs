using Infrastructure.Seedwork.Validation;

namespace Infrastructure.Application.Commands;

public class CommandHandlerResult
{
    public static readonly CommandHandlerResult Unit = new(value: new object(), error: null);

    public bool IsFailure => _error is not null;

    private readonly SystemError? _error;
    public SystemError Error => _error ?? throw new InvalidOperationException("Cannot get error, result is in success state");

    private readonly object? _value;
    public object Value => _value ?? throw new InvalidOperationException("Cannot get value, result is in error state");

    private CommandHandlerResult(object? value, SystemError? error)
    {
        _value = value;
        _error = error;
    }

    public static CommandHandlerResult Success(object result)
    {
        return new CommandHandlerResult(result, error: null);
    }

    public static CommandHandlerResult Failure(SystemError error)
    {
        return new CommandHandlerResult(value: null, error: error);
    }
}