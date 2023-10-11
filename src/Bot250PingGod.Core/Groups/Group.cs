using CSharpFunctionalExtensions;

namespace Bot250PingGod.Core.Groups;

public class GroupConfiguration
{
    public virtual required bool AllowGrowPussyCommand { get; init; }

    public virtual required decimal GrowPussyMinSize { get; init; }

    public virtual required decimal GrowPussyMaxSize { get; init; }

    public static GroupConfiguration CreateDefault()
    {
        return new GroupConfiguration
        {
            AllowGrowPussyCommand = true,
            GrowPussyMinSize      = -10,
            GrowPussyMaxSize      = 10
        };
    }
}

public class Group : Entity
{
    public virtual string? Title { get; protected init; }

    public virtual long ChatId { get; protected init; }

    public virtual GroupConfiguration Configuration { get; protected init; } = null!;

    public static Group Create(string? title,
                               long chatId,
                               GroupConfiguration configuration)
    {
        return new Group
        {
            Title         = title,
            ChatId        = chatId,
            Configuration = configuration
        };
    }
}