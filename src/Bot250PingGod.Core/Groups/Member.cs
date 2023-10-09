using CSharpFunctionalExtensions;

namespace Bot250PingGod.Core.Groups;

public class Member : Entity
{
    public virtual bool IsDeleted { get; protected init; }

    public virtual string? Username { get; protected init; }

    public virtual string FirstName { get; protected init; } = null!;

    public virtual string? LastName { get; protected init; }

    public virtual long UserId { get; protected init; }

    protected Member()
    {
    }

    public static Member Create(string? username,
                                string firstName,
                                string? lastName,
                                long userId)
    {
        return new Member
        {
            IsDeleted = false,
            Username  = username,
            FirstName = firstName,
            LastName  = lastName,
            UserId    = userId,
        };
    }
}