using CSharpFunctionalExtensions;

namespace Bot250PingGod.Core.Rooms;

public class EasterEgg : Entity
{
    public virtual string Command { get; protected init; } = null!;

    public virtual string AnswerText { get; protected init; } = null!;
}