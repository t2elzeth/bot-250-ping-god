using Autofac;
using JetBrains.Annotations;

namespace Bot250PingGod.Application.Rooms;

[UsedImplicitly]
public class CompositionRoot : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<QuestionRepository>().SingleInstance();
        builder.RegisterType<EasterEggRepository>().SingleInstance();
    }
}