using Bot250PingGod.Core.Group;
using FluentNHibernate.Mapping;
using Infrastructure.Application;
using JetBrains.Annotations;

namespace Bot250PingGod.Application.Nh;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public sealed class GroupMemberMap : ClassMap<GroupMember>
{
    public GroupMemberMap()
    {
        Schema(DatabaseSchemas.Bot);

        Id(x => x.Id);

        Map(x => x.IsDeleted);

        Map(x => x.AnabruhateCount);

        Map(x => x.LastAnabruhateDateTime);

        Map(x => x.LastHourAnabruhateCount);

        References(x => x.Group);

        References(x => x.Member);

        References(x => x.Pussy);
    }
}