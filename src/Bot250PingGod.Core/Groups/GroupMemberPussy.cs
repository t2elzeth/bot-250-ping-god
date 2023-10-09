using CSharpFunctionalExtensions;
using Infrastructure.Seedwork.DataTypes;

namespace Bot250PingGod.Core.Groups;

public class GroupMemberPussy : Entity
{
    public virtual decimal Size { get; protected set; }

    public virtual UtcDateTime LastGrowDateTime { get; protected set; } = null!;

    protected GroupMemberPussy()
    {
    }

    public static GroupMemberPussy Create(UtcDateTime dateTime)
    {
        return new GroupMemberPussy
        {
            Size             = 0,
            LastGrowDateTime = dateTime - TimeSpan.FromHours(2)
        };
    }

    public virtual bool CanGrowPussy(UtcDateTime dateTime, out long tryAgainAfterMinutes)
    {
        var diff = dateTime.Value - LastGrowDateTime.Value;

        tryAgainAfterMinutes = 30 - diff.Minutes;

        return diff.TotalMinutes >= 30;
    }

    public virtual decimal GrowPussy(UtcDateTime dateTime)
    {
        const decimal minGrowSize = -10m; // Minimum value of the range
        const decimal maxGrowSize = 10m;  // Maximum value of the range

        var random   = new Random();
        var growSize = (decimal)random.NextDouble() * (maxGrowSize - minGrowSize) + minGrowSize;

        Size             = Math.Round(Size + growSize, 2);
        LastGrowDateTime = dateTime;

        return growSize;
    }
}