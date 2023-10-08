using CSharpFunctionalExtensions;

namespace Bot250PingGod.Core.Group;

public class Group : Entity
{
    public virtual string? Title { get; protected init; }

    public virtual long ChatId { get; protected init; }

    public static Group Create(string? title,
                               long chatId)
    {
        return new Group
        {
            Title  = title,
            ChatId = chatId
        };
    }
}