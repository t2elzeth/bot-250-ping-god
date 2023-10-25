using CSharpFunctionalExtensions;
using Infrastructure.Seedwork.DataTypes;

namespace Bot250PingGod.Core.Groups;

public class GroupMemberPing : Entity
{
    public virtual decimal Ping { get; protected internal set; }

    public virtual UtcDateTime LastPingDateTime { get; protected internal set; } = null!;

    public virtual UtcDateTime? LastLimitNotificationDateTime { get; protected internal set; }

    protected GroupMemberPing()
    {
    }

    public static GroupMemberPing Create(UtcDateTime dateTime)
    {
        return new GroupMemberPing
        {
            Ping                          = 0,
            LastPingDateTime              = dateTime - TimeSpan.FromHours(2),
            LastLimitNotificationDateTime = null
        };
    }
}