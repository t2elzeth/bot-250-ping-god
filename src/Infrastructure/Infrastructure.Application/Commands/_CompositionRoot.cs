using Autofac;
using JetBrains.Annotations;

namespace Infrastructure.Application.Commands;

[UsedImplicitly]
internal class CompositionRoot : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterHandlerDecorator(typeof(ApplicationCommandHandlerDecorator<>));
        builder.RegisterType<CommandHandler>().SingleInstance();
    }
}