using Autofac;
using JetBrains.Annotations;

namespace Bot250PingGod.Application.Commands;

[UsedImplicitly]
public class CompositionRoot : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<TelegramCommandHandler>().SingleInstance();
        builder.RegisterType<EasterEggHandler>().SingleInstance();

        builder.RegisterType<AskTelegramCommandHandler>()
               .Keyed<ITelegramCommandHandler>(TelegramCommand.AskCommand);
    }
}