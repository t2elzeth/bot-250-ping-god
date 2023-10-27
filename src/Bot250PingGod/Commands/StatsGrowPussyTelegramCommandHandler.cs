using Bot250PingGod.Groups;
using Dapper;
using Infrastructure.DataAccess;
using Infrastructure.Seedwork.Providers;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace Bot250PingGod.Commands;

public sealed class StatsGrowPussyTelegramCommandHandler : ITelegramCommandHandler
{
    private readonly ILogger<StatsGrowPussyTelegramCommandHandler> _logger;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ITelegramBotClient _botClient;
    private readonly GroupRepository _groupRepository;

    public StatsGrowPussyTelegramCommandHandler(ILogger<StatsGrowPussyTelegramCommandHandler> logger,
                                                IDateTimeProvider dateTimeProvider,
                                                ITelegramBotClient botClient,
                                                GroupRepository groupRepository)
    {
        _logger           = logger;
        _dateTimeProvider = dateTimeProvider;
        _botClient        = botClient;
        _groupRepository  = groupRepository;
    }

    public async Task HandleAsync(TelegramCommand command, CancellationToken cancellationToken)
    {
        var now     = _dateTimeProvider.Now();
        var message = command.Message;
        var chatId  = message.Chat.Id;

        var group         = await _groupRepository.GetByChatIdAsync(chatId, cancellationToken);
        var configuration = group.Configuration;

        if (!configuration.AllowGrowPussyCommand)
        {
            _logger.LogWarning("Cannot handle statsgrowpussy command, this command is disabled for Group#{ChatId}",
                               message.Chat.Id);

            await _botClient.DeleteMessageAsync(chatId: message.Chat.Id,
                                                messageId: message.MessageId,
                                                cancellationToken: cancellationToken);

            return;
        }

        //language=sql
        const string sql = @"
select row_number() over (order by t.size asc) as row_number,
       coalesce(m.username, m.first_name) as name,
       t.size
  from bot.group_member_pussies t
  inner join bot.group_members gm on gm.pussy_id = t.id
  inner join bot.groups g on g.id = gm.group_id
  inner join bot.members m on m.id = gm.member_id
 where g.chat_id = :groupId
   and t.size != 0
   and t.last_grow_date_time >= :lastGrowDateTimeLaterThan;
";

        var dbSession  = DbSession.Current;
        var connection = dbSession.Connection;

        var parameters = new
        {
            groupId                   = message.Chat.Id,
            lastGrowDateTimeLaterThan = now - TimeSpan.FromDays(7)
        };

        var commandDefinition = new CommandDefinition(sql, parameters, cancellationToken: cancellationToken);
        var groupMembers      = await connection.QueryAsync<GroupMemberRow>(commandDefinition);

        var groupMembersCountArray = groupMembers.Select(x => $"{x.RowNumber}. {x.Name}: {Math.Round(x.Size, 2)} см");

        var messageText = $"Статистика пусси: \n{string.Join('\n', groupMembersCountArray)}";

        await _botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                              text: messageText,
                                              cancellationToken: cancellationToken);
    }

    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    private sealed class GroupMemberRow
    {
        public long RowNumber { get; init; }

        public string Name { get; init; } = null!;

        public decimal Size { get; init; }
    }
}