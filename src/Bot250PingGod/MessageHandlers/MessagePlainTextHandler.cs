using Dapper;
using Infrastructure.DataAccess;
using NHibernate;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Bot250PingGod.MessageHandlers;

public sealed class MessagePlainTextHandler
{
    private readonly ISessionFactory _sessionFactory;
    private readonly ITelegramBotClient _botClient;

    public MessagePlainTextHandler(ISessionFactory sessionFactory,
                                   ITelegramBotClient botClient)
    {
        _sessionFactory = sessionFactory;
        _botClient      = botClient;
    }

    public async Task HandleAsync(Message message, CancellationToken cancellationToken)
    {
        var chatId      = message.Chat.Id;
        var messageText = message.Text;

        if (messageText is null)
            return;

        const string sql = @"
select lower(t.message_text) as message_text,
       t.answer_text,
       t.min_similarity
  from bot.plain_message_answers t
 where t.is_disabled = false;
";

        IEnumerable<MessageRow> rows;
        await using (DbSession.Bind(_sessionFactory))
        {
            var dbSession  = DbSession.Current;
            var connection = dbSession.Connection;

            rows = await connection.QueryAsync<MessageRow>(sql);
        }

        foreach (var row in rows)
        {
            var similarity = (decimal)JaroWinklerDistance.Proximity(messageText.ToLower(), row.MessageText);
            if (similarity <= row.MinSimilarity)
                continue;

            await _botClient.SendTextMessageAsync(chatId: chatId,
                                                  text: row.AnswerText,
                                                  replyToMessageId: message.MessageId,
                                                  cancellationToken: cancellationToken);

            break;
        }
    }

    private sealed class MessageRow
    {
        public string MessageText { get; init; } = null!;

        public string AnswerText { get; init; } = null!;
        
        public decimal MinSimilarity { get; init; }
    }
}