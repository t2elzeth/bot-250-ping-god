using CSharpFunctionalExtensions;

namespace Bot250PingGod.Core.Rooms;

public class Player : Entity
{
    public virtual Room Room { get; protected init; } = null!;

    public virtual string Username { get; protected init; } = null!;
}