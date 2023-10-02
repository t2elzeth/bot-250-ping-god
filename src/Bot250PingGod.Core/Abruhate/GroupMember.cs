using CSharpFunctionalExtensions;

namespace Bot250PingGod.Core.Abruhate;

public class GroupMember : Entity
{
    public virtual bool IsDeleted { get; protected init; }

    public virtual string Username { get; protected init; } = null!;

    public virtual long AnabruhateCount { get; protected set; }

    protected GroupMember()
    {
    }

    public static GroupMember Create(string username)
    {
        return new GroupMember
        {
            IsDeleted       = false,
            Username        = username,
            AnabruhateCount = 0
        };
    }

    public virtual void Anabruhate()
    {
        AnabruhateCount++;
    }
}