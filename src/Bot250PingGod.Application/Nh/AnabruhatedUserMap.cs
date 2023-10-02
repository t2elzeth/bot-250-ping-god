using Bot250PingGod.Core.Abruhate;
using FluentNHibernate.Mapping;
using Infrastructure.Application;
using JetBrains.Annotations;

namespace Bot250PingGod.Application.Nh;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public sealed class AnabruhatedUserMap : ClassMap<AnabruhatedUser>
{
    public AnabruhatedUserMap()
    {
        Schema(DatabaseSchemas.Bot);

        Id(x => x.Id);

        Map(x => x.Username);
    }
}