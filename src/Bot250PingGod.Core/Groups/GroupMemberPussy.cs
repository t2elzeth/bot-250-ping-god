using CSharpFunctionalExtensions;
using Infrastructure.Seedwork.DataTypes;

namespace Bot250PingGod.Core.Groups;

public class GroupMemberPussy : Entity
{
    public virtual decimal Size { get; protected set; }

    public virtual UtcDateTime LastGrowDateTime { get; protected set; } = null!;

    public virtual UtcDateTime? LastLimitNotificationDateTime { get; protected set; }

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

    public virtual bool CanGrow(UtcDateTime dateTime, out long tryAgainAfterMinutes)
    {
        var diff = dateTime.Value - LastGrowDateTime.Value;

        tryAgainAfterMinutes = 30 - diff.Minutes;

        return diff.TotalMinutes >= 30;
    }

    public virtual bool ShouldNotifyLimit(UtcDateTime dateTime)
    {
        if (LastLimitNotificationDateTime is null)
            return true;

        var lastLimitNotificationDiff = dateTime.Value - LastLimitNotificationDateTime.Value;

        return lastLimitNotificationDiff.TotalMinutes >= 10;
    }

    public virtual void NotifyLimit(UtcDateTime dateTime)
    {
        LastLimitNotificationDateTime = dateTime;
    }

    public virtual decimal Grow(UtcDateTime dateTime,
                                decimal minGrowSize,
                                decimal maxGrowSize)
    {
        var random   = new Random();
        var growSize = (decimal)random.NextDouble() * (maxGrowSize - minGrowSize) + minGrowSize;

        Size                          = Math.Round(Size + growSize, 2);
        LastGrowDateTime              = dateTime;
        LastLimitNotificationDateTime = null;

        return growSize;
    }
}