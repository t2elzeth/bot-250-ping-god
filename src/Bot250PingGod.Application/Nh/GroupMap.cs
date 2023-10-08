using Bot250PingGod.Core.Group;
using FluentNHibernate.Mapping;
using Infrastructure.Application;
using JetBrains.Annotations;

namespace Bot250PingGod.Application.Nh;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public sealed class GroupMap : ClassMap<Group>
{
    public GroupMap()
    {
        Schema(DatabaseSchemas.Bot);
        
        Id(x => x.Id);

        Map(x => x.ChatId);

        Map(x => x.Title);
    }
}