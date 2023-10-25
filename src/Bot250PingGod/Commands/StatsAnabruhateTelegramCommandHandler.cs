using Dapper;
using Infrastructure.DataAccess;
using JetBrains.Annotations;
using Telegram.Bot;

namespace Bot250PingGod.Commands;

public sealed class StatsAnabruhateTelegramCommandHandler : ITelegramCommandHandler
{
    private readonly ITelegramBotClient _botClient;

    public StatsAnabruhateTelegramCommandHandler(ITelegramBotClient botClient)
    {
        _botClient = botClient;
    }

    public async Task HandleAsync(TelegramCommand command, CancellationToken cancellationToken)
    {
        //language=sql
        const string sql = @"
select row_number() over (order by t.anabruhate_count desc) as row_number,
       t.username,
       t.anabruhate_count
from bot.group_members t;
";

        var dbSession  = DbSession.Current;
        var connection = dbSession.Connection;

        var groupMembers = await connection.QueryAsync<GroupMemberRow>(sql, cancellationToken);

        var groupMembersCountArray = groupMembers.Select(x => $"{x.RowNumber}. {x.Username} - {x.AnabruhateCount}");

        var messageText = $"Статистика анабрюхативания: \n{string.Join('\n', groupMembersCountArray)}";

        await _botClient.SendTextMessageAsync(chatId: command.Message.Chat.Id,
                                              text: messageText,
                                              cancellationToken: cancellationToken);
    }

    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    private sealed class GroupMemberRow
    {
        public long RowNumber { get; init; }

        public string Username { get; init; } = null!;

        public long AnabruhateCount { get; init; }
    }
}