using Autofac;
using JetBrains.Annotations;

namespace Infrastructure.Application.DomainEvents;

[UsedImplicitly]
internal sealed class CompositionRoot : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<DomainEventsHandler>().SingleInstance();
    }
}