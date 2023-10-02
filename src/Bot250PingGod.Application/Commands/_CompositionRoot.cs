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

        builder.RegisterType<AnabruhateTelegramCommandHandler>()
               .Keyed<ITelegramCommandHandler>("/anabruhate");
        
        builder.RegisterType<AnabruhateTelegramCommandHandler>()
               .Keyed<ITelegramCommandHandler>("/anabruhate@t2_250_ping_god_bot");
    }
}