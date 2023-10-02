using Autofac;

namespace Infrastructure.Application.Commands;

public static class CommandHandlerAutofacExtensions
{
    public static void RegisterHandler<TCommand, THandler>(this ContainerBuilder serviceRegistry)
        where THandler : ICommandHandler<TCommand>
    {
        serviceRegistry.RegisterType<THandler>()
                       .As<ICommandHandler<TCommand>>()
                       .SingleInstance();
    }

    public static void RegisterHandlerDecorator(this ContainerBuilder serviceRegistry,
                                                Type decoratorType)
    {
        serviceRegistry.RegisterGeneric(decoratorType)
                       .SingleInstance();
    }
}
