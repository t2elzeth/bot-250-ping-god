using Autofac;
using JetBrains.Annotations;

namespace Bot250PingGod.Application.Commands;

[UsedImplicitly]
public class CompositionRoot : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<TelegramCommandHandler>().SingleInstance();

        builder.RegisterType<PussyTelegramCommandHandler>()
               .Keyed<ITelegramCommandHandler>("/growpussy");

        builder.RegisterType<PussyTelegramCommandHandler>()
               .Keyed<ITelegramCommandHandler>("/growpussy@t2_250_ping_god_bot");
        
        builder.RegisterType<PussyStatsTelegramCommandHandler>()
               .Keyed<ITelegramCommandHandler>("/statsgrowpussy");

        builder.RegisterType<PussyStatsTelegramCommandHandler>()
               .Keyed<ITelegramCommandHandler>("/statsgrowpussy@t2_250_ping_god_bot");
    }
}