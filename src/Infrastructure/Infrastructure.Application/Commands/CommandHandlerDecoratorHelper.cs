using Autofac;

namespace Infrastructure.Application.Commands;

internal static class CommandHandlerDecoratorHelper
{
    public static ICommandHandler<TCommand> Decorate<TCommand>(ILifetimeScope serviceFactory,
                                                               Type commandHandlerType,
                                                               ICommandHandler<TCommand> executingCommandHandler)
    {
        var decoratorTypes = commandHandlerType.GetCustomAttributes(inherit: true)
                                               .OfType<CommandHandlerDecoratorAttribute>()
                                               .Select(a => a.DecoratorType)
                                               .ToList();

        var resultHandler = executingCommandHandler;

        for (var i = decoratorTypes.Count - 1; i >= 0; i--)
        {
            var decoratorType        = decoratorTypes[i];
            var genericDecoratorType = decoratorType.MakeGenericType(typeof(TCommand));

            var nextParameter = new TypedParameter(typeof(ICommandHandler<TCommand>), resultHandler);
            resultHandler = (ICommandHandler<TCommand>)serviceFactory.Resolve(genericDecoratorType, nextParameter);
        }

        return resultHandler;
    }
}