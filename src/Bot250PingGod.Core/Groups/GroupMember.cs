using CSharpFunctionalExtensions;
using Infrastructure.Seedwork.DataTypes;

namespace Bot250PingGod.Core.Groups;

public class GroupMember : Entity
{
    public virtual long AnabruhateCount { get; protected set; }

    public virtual UtcDateTime LastAnabruhateDateTime { get; protected set; } = null!;

    public virtual long LastHourAnabruhateCount { get; protected set; }

    public virtual Group Group { get; protected set; } = null!;

    public virtual Member Member { get; protected set; } = null!;

    public virtual GroupMemberPussy Pussy { get; protected init; } = null!;

    public virtual GroupMemberPing? Ping { get; protected set; }

    protected GroupMember()
    {
    }

    public static GroupMember Create(UtcDateTime dateTime,
                                     Group group,
                                     Member member)
    {
        return new GroupMember
        {
            AnabruhateCount        = 0,
            LastAnabruhateDateTime = dateTime,
            Group                  = group,
            Member                 = member,
            Pussy                  = GroupMemberPussy.Create(dateTime),
            Ping                   = GroupMemberPing.Create(dateTime)
        };
    }

    public virtual decimal GrowPussy(UtcDateTime dateTime)
    {
        var configuration = Group.Configuration;
        var minGrowSize   = configuration.GrowPussyMinSize;
        var maxGrowSize   = configuration.GrowPussyMaxSize;

        var random   = new Random();
        var growSize = (decimal)random.NextDouble() * (maxGrowSize - minGrowSize) + minGrowSize;

        Pussy.Size                          = Math.Round(Pussy.Size + growSize, 2);
        Pussy.LastGrowDateTime              = dateTime;
        Pussy.LastLimitNotificationDateTime = null;

        return growSize;
    }

    public virtual bool CanGrowPussy(UtcDateTime dateTime, out long tryAgainAfterMinutes)
    {
        var diff = dateTime.Value - Pussy.LastGrowDateTime.Value;

        tryAgainAfterMinutes = 15 - diff.Minutes;

        return diff.TotalMinutes >= 15;
    }

    public virtual bool ShouldNotifyPussyLimit(UtcDateTime dateTime)
    {
        if (Pussy.LastLimitNotificationDateTime is null)
            return true;

        var lastLimitNotificationDiff = dateTime.Value - Pussy.LastLimitNotificationDateTime.Value;

        return lastLimitNotificationDiff.TotalMinutes >= 3;
    }

    public virtual void NotifyPussyLimit(UtcDateTime dateTime)
    {
        Pussy.LastLimitNotificationDateTime = dateTime;
    }

    public virtual GroupMemberPing GetPing(UtcDateTime dateTime)
    {
        Ping ??= GroupMemberPing.Create(dateTime);

        return Ping;
    }

    public virtual decimal DoPing(UtcDateTime dateTime)
    {
        const int minGrowSize = -250;
        const int maxGrowSize = +250;

        var random   = new Random();
        var growSize = (decimal)random.NextDouble() * (maxGrowSize - minGrowSize) + minGrowSize;

        Ping ??= GroupMemberPing.Create(dateTime);

        Ping.Ping                          = Math.Round(Ping.Ping + growSize, 2);
        Ping.LastPingDateTime              = dateTime;
        Ping.LastLimitNotificationDateTime = null;

        return growSize;
    }

    public virtual bool CanPing(UtcDateTime dateTime, out long tryAgainAfterMinutes)
    {
        Ping ??= GroupMemberPing.Create(dateTime);

        var diff = dateTime.Value - Ping.LastPingDateTime.Value;

        tryAgainAfterMinutes = 3 - diff.Minutes;

        return diff.TotalMinutes >= 3;
    }

    public virtual bool ShouldNotifyPingLimit(UtcDateTime dateTime)
    {
        Ping ??= GroupMemberPing.Create(dateTime);

        if (Ping.LastLimitNotificationDateTime is null)
            return true;

        var lastLimitNotificationDiff = dateTime.Value - Ping.LastLimitNotificationDateTime.Value;

        return lastLimitNotificationDiff.TotalMinutes >= 1;
    }

    public virtual void NotifyPingLimit(UtcDateTime dateTime)
    {
        Ping ??= GroupMemberPing.Create(dateTime);

        Ping.LastLimitNotificationDateTime = dateTime;
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