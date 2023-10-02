using Dapper;
using Infrastructure.DataAccess;
using JetBrains.Annotations;
using Telegram.Bot;

namespace Bot250PingGod.Application.Commands;

public sealed class StatsTelegramCommandHandler : ITelegramCommandHandler
{
    private readonly ITelegramBotClient _botClient;

    public StatsTelegramCommandHandler(ITelegramBotClient botClient)
    {
        _botClient = botClient;
    }

    public async Task HandleAsync(TelegramCommand command, CancellationToken cancellationToken)
    {
        //language=sql
        const string sql = @"
select t.username,
       t.anabruhate_count
  from bot.group_members t 
 order by t.anabruhate_count desc;
";

        var dbSession  = DbSession.Current;
        var connection = dbSession.Connection;

        var groupMembers = await connection.QueryAsync<GroupMemberRow>(sql, cancellationToken);

        var groupMembersCountArray = groupMembers.Select(x => $"{x.Username} - {x.AnabruhateCount}");

        var messageText = $"Статистика анабрюхативания: \n{string.Join('\n', groupMembersCountArray)}";

        await _botClient.SendTextMessageAsync(chatId: command.ChatId,
                                              text: messageText,
                                              cancellationToken: cancellationToken);
    }

    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    private sealed class GroupMemberRow
    {
        public string Username { get; init; } = null!;

        public long AnabruhateCount { get; init; }
    }
}