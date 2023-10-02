using CSharpFunctionalExtensions;

namespace Bot250PingGod.Core.Rooms;

public class Question : Entity
{
    public virtual string Text { get; set; } = null!;
}