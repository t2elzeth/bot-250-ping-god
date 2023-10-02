using Dapper;
using Infrastructure.DataAccess;
using Telegram.Bot;

namespace Bot250PingGod.Application.Commands;

public sealed class AnabruhateTelegramCommandHandler : ITelegramCommandHandler
{
    private readonly ITelegramBotClient _botClient;

    public AnabruhateTelegramCommandHandler(ITelegramBotClient botClient)
    {
        _botClient = botClient;
    }

    public async Task HandleAsync(TelegramCommand command, CancellationToken cancellationToken)
    {
        //language=sql
        const string sql = @"
select t.username
  from bot.anabruhated_users t 
 order by random()
 limit 1;
";

        var dbSession  = DbSession.Current;
        var connection = dbSession.Connection;

        var username = await connection.ExecuteScalarAsync<string>(sql, cancellationToken);

        var messageText = $"@{username} пошел нахуй";

        await _botClient.SendTextMessageAsync(chatId: command.ChatId,
                                              text: messageText,
                                              cancellationToken: cancellationToken);
    }
}