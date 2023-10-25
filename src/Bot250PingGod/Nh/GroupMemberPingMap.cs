using Bot250PingGod.Core.Groups;
using FluentNHibernate.Mapping;
using Infrastructure.Application;
using JetBrains.Annotations;

namespace Bot250PingGod.Nh;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public sealed class GroupMemberPingMap : ClassMap<GroupMemberPing>
{
    public GroupMemberPingMap()
    {
        Schema(DatabaseSchemas.Bot);

        Id(x => x.Id);

        Map(x => x.Ping);

        Map(x => x.LastPingDateTime);

        Map(x => x.LastLimitNotificationDateTime);
    }
}