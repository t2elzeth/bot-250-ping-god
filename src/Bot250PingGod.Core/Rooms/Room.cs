using CSharpFunctionalExtensions;

namespace Bot250PingGod.Core.Rooms;

public class Room : Entity
{
    public virtual string Name { get; protected init; } = null!;

    public virtual RoomState State { get; protected set; } = null!;
}