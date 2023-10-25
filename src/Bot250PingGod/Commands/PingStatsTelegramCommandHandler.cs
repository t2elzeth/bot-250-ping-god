using Dapper;
using Infrastructure.DataAccess;
using Infrastructure.Seedwork.Providers;
using JetBrains.Annotations;
using Telegram.Bot;

namespace Bot250PingGod.Commands;

public sealed class PingStatsTelegramCommandHandler : ITelegramCommandHandler
{
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ITelegramBotClient _botClient;

    public PingStatsTelegramCommandHandler(IDateTimeProvider dateTimeProvider,
                                           ITelegramBotClient botClient)
    {
        _dateTimeProvider = dateTimeProvider;
        _botClient        = botClient;
    }

    public async Task HandleAsync(TelegramCommand command, CancellationToken cancellationToken)
    {
        var now = _dateTimeProvider.Now();

        //language=sql
        const string sql = @"
select row_number() over (order by abs(t.ping)) as row_number,
       coalesce(m.username, m.first_name) as name,
       t.ping
  from bot.group_member_pings t
  inner join bot.group_members gm on gm.ping_id = t.id
  inner join bot.groups g on g.id = gm.group_id
  inner join bot.members m on m.id = gm.member_id
 where g.chat_id = :groupId
   and t.ping != 0
   and t.last_ping_date_time >= :lastPingDateTimeLaterThan;
";

        var dbSession  = DbSession.Current;
        var connection = dbSession.Connection;

        var parameters = new
        {
            groupId                   = command.Message.Chat.Id,
            lastPingDateTimeLaterThan = now - TimeSpan.FromDays(7)
        };

        var commandDefinition = new CommandDefinition(sql, parameters, cancellationToken: cancellationToken);
        var groupMembers      = await connection.QueryAsync<Row>(commandDefinition);

        var groupMembersCountArray = groupMembers.Select(x => $"{x.RowNumber}. {x.Name}: {Math.Round(x.Ping, 2)} мс");

        var messageText = $"Статистика пинга: \n{string.Join('\n', groupMembersCountArray)}";

        await _botClient.SendTextMessageAsync(chatId: command.Message.Chat.Id,
                                              text: messageText,
                                              cancellationToken: cancellationToken);
    }

    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    private sealed class Row
    {
        public long RowNumber { get; init; }

        public string Name { get; init; } = null!;

        public decimal Ping { get; init; }
    }
}