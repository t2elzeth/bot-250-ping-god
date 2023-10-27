using Autofac;
using JetBrains.Annotations;

namespace Bot250PingGod.MessageHandlers;

[UsedImplicitly]
public class CompositionRoot : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<MessagePlainTextHandler>().SingleInstance();
        builder.RegisterType<StickerHandler>().SingleInstance();
    }
}