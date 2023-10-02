using Bot250PingGod.Core.Rooms;
using FluentNHibernate.Mapping;
using JetBrains.Annotations;

namespace Bot250PingGod.Application.Nh;

[UsedImplicitly]
public sealed class EasterEggMap : ClassMap<EasterEgg>
{
    public EasterEggMap()
    {
        Id(x => x.Id);

        Map(x => x.Command);

        Map(x => x.AnswerText);
    }
}