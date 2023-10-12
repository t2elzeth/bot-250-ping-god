﻿using Dapper;
using Infrastructure.DataAccess;
using JetBrains.Annotations;
using Telegram.Bot;

namespace Bot250PingGod.Application.Commands;

public sealed class GrowPussyStatsTelegramCommandHandler : ITelegramCommandHandler
{
    private readonly ITelegramBotClient _botClient;

    public GrowPussyStatsTelegramCommandHandler(ITelegramBotClient botClient)
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
  inner join bot.groups g on g.id = gm.group_id
  inner join bot.members m on m.id = gm.member_id
 where g.chat_id = :groupId;
";

        var dbSession  = DbSession.Current;
        var connection = dbSession.Connection;

        var parameters = new
        {
            groupId = command.Message.Chat.Id
        };

        var commandDefinition = new CommandDefinition(sql, parameters, cancellationToken: cancellationToken);
        var groupMembers      = await connection.QueryAsync<GroupMemberRow>(commandDefinition);

        var groupMembersCountArray = groupMembers.Select(x => $"{x.RowNumber}. {x.Name} - {Math.Round(x.Size, 2)} см");

        var messageText = $"Статистика пусси: \n{string.Join('\n', groupMembersCountArray)}";

        await _botClient.SendTextMessageAsync(chatId: command.Message.Chat.Id,
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