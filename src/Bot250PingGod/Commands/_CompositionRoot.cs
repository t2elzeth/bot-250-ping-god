using Autofac;
using JetBrains.Annotations;

namespace Bot250PingGod.Commands;

[UsedImplicitly]
public class CompositionRoot : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<TelegramCommandHandler>().SingleInstance();

        builder.RegisterType<GrowPussyTelegramCommandHandler>()
               .Keyed<ITelegramCommandHandler>("/growpussy");
        builder.RegisterType<GrowPussyTelegramCommandHandler>()
               .Keyed<ITelegramCommandHandler>("/growpussy@t2_250_ping_god_bot");

        builder.RegisterType<CheckPingTelegramCommandHandler>()
               .Keyed<ITelegramCommandHandler>("/checkping");
        builder.RegisterType<CheckPingTelegramCommandHandler>()
               .Keyed<ITelegramCommandHandler>("/checkping@t2_250_ping_god_bot");

        builder.RegisterType<StatsGrowPussyTelegramCommandHandler>()
               .Keyed<ITelegramCommandHandler>("/statsgrowpussy");
        builder.RegisterType<StatsGrowPussyTelegramCommandHandler>()
               .Keyed<ITelegramCommandHandler>("/statsgrowpussy@t2_250_ping_god_bot");

        builder.RegisterType<StatsPingTelegramCommandHandler>()
               .Keyed<ITelegramCommandHandler>("/statsping");
        builder.RegisterType<StatsPingTelegramCommandHandler>()
               .Keyed<ITelegramCommandHandler>("/statsping@t2_250_ping_god_bot");
    }
}