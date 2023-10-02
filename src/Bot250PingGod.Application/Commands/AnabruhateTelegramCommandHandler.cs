using Bot250PingGod.Application.Rooms;
using Dapper;
using Infrastructure.DataAccess;
using Telegram.Bot;

namespace Bot250PingGod.Application.Commands;

public sealed class AnabruhateTelegramCommandHandler : ITelegramCommandHandler
{
    private readonly ITelegramBotClient _botClient;
    private readonly GroupMemberRepository _groupMemberRepository;

    public AnabruhateTelegramCommandHandler(ITelegramBotClient botClient,
                                            GroupMemberRepository groupMemberRepository)
    {
        _botClient             = botClient;
        _groupMemberRepository = groupMemberRepository;
    }

    public async Task HandleAsync(TelegramCommand command, CancellationToken cancellationToken)
    {
        //language=sql
        const string sql = @"
select t.id
  from bot.group_members t 
 order by random()
 limit 1;
";

        var dbSession  = DbSession.Current;
        var connection = dbSession.Connection;

        var anabruhatedUserId = await connection.ExecuteScalarAsync<long>(sql, cancellationToken);

        var groupMember = await _groupMemberRepository.GetAsync(anabruhatedUserId, cancellationToken);

        var messageText = $"@{groupMember.Username} пошел нахуй";

        await _botClient.SendTextMessageAsync(chatId: command.ChatId,
                                              text: messageText,
                                              cancellationToken: cancellationToken);

        groupMember.Anabruhate();

        await _groupMemberRepository.SaveAsync(groupMember, cancellationToken);
    }
}