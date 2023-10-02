using CSharpFunctionalExtensions;

namespace Bot250PingGod.Core.Abruhate;

public class AnabruhatedUser : Entity
{
    public virtual string Username { get; protected init; } = null!;

    protected AnabruhatedUser()
    {
    }

    public static AnabruhatedUser Create(string username)
    {
        return new AnabruhatedUser
        {
            Username = username
        };
    }
}