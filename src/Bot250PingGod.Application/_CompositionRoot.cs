using System.Reflection;
using Autofac;
using JetBrains.Annotations;
using Module = Autofac.Module;

namespace Bot250PingGod.Application;

[UsedImplicitly]
public class CompositionRoot : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<TelegramBot>().SingleInstance();

        builder.RegisterAssemblyModules(Assembly.Load("Infrastructure.SeedWork"));
    }
}