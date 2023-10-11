﻿using Autofac;
using JetBrains.Annotations;

namespace Bot250PingGod.Application.Commands;

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
        
        builder.RegisterType<GrowPussyStatsTelegramCommandHandler>()
               .Keyed<ITelegramCommandHandler>("/statsgrowpussy");

        builder.RegisterType<GrowPussyStatsTelegramCommandHandler>()
               .Keyed<ITelegramCommandHandler>("/statsgrowpussy@t2_250_ping_god_bot");
    }
}