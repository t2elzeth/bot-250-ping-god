using Dapper;
using Infrastructure.DataAccess;
using JetBrains.Annotations;
using Telegram.Bot;

namespace Bot250PingGod.Application.Commands;

public sealed class PussyStatsTelegramCommandHandler : ITelegramCommandHandler
{
    private readonly ITelegramBotClient _botClient;

    public PussyStatsTelegramCommandHandler(ITelegramBotClient botClient)
    {
        _botClient = botClient;
    }

    public async Task HandleAsync(TelegramCommand command, CancellationToken cancellationToken)
    {
        //language=sql
        const string sql = @"
select row_number() over (order by t.size desc) as row_number,
       coalesce(m.username, m.first_name) as name,
       t.size
  from bot.group_member_pussies t
  inner join bot.group_members gm on gm.pussy_id = t.id
  inner join bot.members m on m.id = gm.member_id;
";

        var dbSession  = DbSession.Current;
        var connection = dbSession.Connection;

        var groupMembers = await connection.QueryAsync<GroupMemberRow>(sql, cancellationToken);

        var groupMembersCountArray = groupMembers.Select(x => $"{x.RowNumber}. {x.Username} - {x.AnabruhateCount}");

        var messageText = $"Статистика пусси: \n{string.Join('\n', groupMembersCountArray)}";

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