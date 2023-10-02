using Bot250PingGod.Core.Rooms;
using FluentNHibernate.Mapping;
using JetBrains.Annotations;

namespace Bot250PingGod.Application.Nh;

[UsedImplicitly]
public sealed class QuestionMap : ClassMap<Question>
{
    public QuestionMap()
    {
        Id(x => x.Id);

        Map(x => x.Text);
    }
}