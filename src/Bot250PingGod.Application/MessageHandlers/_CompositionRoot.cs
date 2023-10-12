using Autofac;
using JetBrains.Annotations;

namespace Bot250PingGod.Application.MessageHandlers;

[UsedImplicitly]
public class CompositionRoot : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<MessagePlainTextHandler>().SingleInstance();
    }
}