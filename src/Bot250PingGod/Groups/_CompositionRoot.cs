using Autofac;
using JetBrains.Annotations;

namespace Bot250PingGod.Groups;

[UsedImplicitly]
public class CompositionRoot : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<GroupMemberRepository>().SingleInstance();
        builder.RegisterType<GroupRepository>().SingleInstance();
        builder.RegisterType<MemberRepository>().SingleInstance();
    }
}