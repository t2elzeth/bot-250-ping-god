using Bot250PingGod.Core.Groups;
using FluentNHibernate.Mapping;
using Infrastructure.Application;
using JetBrains.Annotations;

namespace Bot250PingGod.Application.Nh;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public sealed class GroupConfigurationMap : ComponentMap<GroupConfiguration>
{
    public GroupConfigurationMap()
    {
        Map(x => x.AllowGrowPussyCommand);

        Map(x => x.AllowPingCommand);

        Map(x => x.GrowPussyMinSize);

        Map(x => x.GrowPussyMaxSize);
    }
}

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public sealed class GroupMap : ClassMap<Group>
{
    public GroupMap()
    {
        Schema(DatabaseSchemas.Bot);

        Id(x => x.Id);

        Map(x => x.ChatId);

        Map(x => x.Title);

        Component(x => x.Configuration);
    }
}