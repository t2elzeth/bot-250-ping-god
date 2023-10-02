using Autofac;
using JetBrains.Annotations;

namespace Bot250PingGod.Application;

[UsedImplicitly]
public class CompositionRoot : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<TelegramBot>().SingleInstance();
    }
}