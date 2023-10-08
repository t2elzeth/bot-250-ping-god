using CSharpFunctionalExtensions;
using Infrastructure.Seedwork.DataTypes;

namespace Bot250PingGod.Core.Group;

public class GroupMember : Entity
{
    public virtual bool IsDeleted { get; protected init; }

    public virtual long AnabruhateCount { get; protected set; }

    public virtual UtcDateTime LastAnabruhateDateTime { get; protected set; } = null!;

    public virtual long LastHourAnabruhateCount { get; protected set; }

    public virtual Group Group { get; protected set; } = null!;

    public virtual Member Member { get; protected set; } = null!;

    public virtual GroupMemberPussy Pussy { get; protected init; } = null!;

    protected GroupMember()
    {
    }

    public static GroupMember Create(UtcDateTime dateTime,
                                     Group group,
                                     Member member)
    {
        return new GroupMember
        {
            IsDeleted              = false,
            AnabruhateCount        = 0,
            LastAnabruhateDateTime = dateTime,
            Group                  = group,
            Member                 = member,
            Pussy                  = GroupMemberPussy.Create(dateTime)
        };
    }

    public virtual bool CanAnabruhate()
    {
        return LastHourAnabruhateCount < 3;
    }

    public virtual void Anabruhate()
    {
        AnabruhateCount++;
    }

    public virtual void IncreaseAnabruhateCount()
    {
        LastHourAnabruhateCount++;
    }

    public virtual void UpdateLastAnabruhateDateTime(UtcDateTime dateTime)
    {
        LastAnabruhateDateTime  = dateTime;
        LastHourAnabruhateCount = 0;
    }
}