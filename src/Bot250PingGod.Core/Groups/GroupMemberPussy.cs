using CSharpFunctionalExtensions;
using Infrastructure.Seedwork.DataTypes;

namespace Bot250PingGod.Core.Groups;

public class GroupMemberPussy : Entity
{
    public virtual decimal Size { get; protected internal set; }

    public virtual UtcDateTime LastGrowDateTime { get; protected internal set; } = null!;

    public virtual UtcDateTime? LastLimitNotificationDateTime { get; protected internal set; }

    protected GroupMemberPussy()
    {
    }

    public static GroupMemberPussy Create(UtcDateTime dateTime)
    {
        return new GroupMemberPussy
        {
            Size                          = 0,
            LastGrowDateTime              = dateTime - TimeSpan.FromHours(2),
            LastLimitNotificationDateTime = null
        };
    }
}