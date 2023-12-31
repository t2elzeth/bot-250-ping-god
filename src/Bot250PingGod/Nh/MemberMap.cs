﻿using Bot250PingGod.Core.Groups;
using FluentNHibernate.Mapping;
using Infrastructure.Application;
using JetBrains.Annotations;

namespace Bot250PingGod.Nh;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public sealed class MemberMap : ClassMap<Member>
{
    public MemberMap()
    {
        Schema(DatabaseSchemas.Bot);

        Id(x => x.Id);

        Map(x => x.Username);

        Map(x => x.FirstName);

        Map(x => x.LastName);

        Map(x => x.UserId);
    }
}