using Bot250PingGod.Core.Groups;
using FluentNHibernate.Mapping;
using Infrastructure.Application;
using JetBrains.Annotations;

namespace Bot250PingGod.Application.Nh;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public sealed class GroupMemberPussyMap : ClassMap<GroupMemberPussy>
{
    public GroupMemberPussyMap()
    {
        Schema(DatabaseSchemas.Bot);

        Id(x => x.Id);

        Map(x => x.Size);

        Map(x => x.LastGrowDateTime);
    }
}