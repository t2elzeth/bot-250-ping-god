using Bot250PingGod.Core.Groups;
using FluentNHibernate.Mapping;
using Infrastructure.Application;
using JetBrains.Annotations;

namespace Bot250PingGod.Nh;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public sealed class GroupMemberMap : ClassMap<GroupMember>
{
    public GroupMemberMap()
    {
        Schema(DatabaseSchemas.Bot);

        Id(x => x.Id);

        Map(x => x.AnabruhateCount);

        Map(x => x.LastAnabruhateDateTime);

        Map(x => x.LastHourAnabruhateCount);

        References(x => x.Group);

        References(x => x.Member);

        References(x => x.Pussy);

        References(x => x.Ping);
    }
}