using Autofac;
using JetBrains.Annotations;

namespace Infrastructure.Seedwork.Providers;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class CompositionRoot : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<DateTimeProvider>().As<IDateTimeProvider>().SingleInstance();
        
        builder.RegisterType<SystemEnvironment>()
               .UsingConstructor(typeof(SystemEnvironmentConfiguration))
               .SingleInstance();
    }
}