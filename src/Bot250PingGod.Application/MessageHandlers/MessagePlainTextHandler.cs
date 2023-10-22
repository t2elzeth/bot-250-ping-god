using Telegram.Bot;
using Telegram.Bot.Types;

namespace Bot250PingGod.Application.MessageHandlers;

public sealed class MessagePlainTextHandler
{
    private readonly ITelegramBotClient _botClient;

    private readonly Dictionary<string, string> _messageAnswers = new()
    {
        { "да", "пизда" },
        { "нет", "пидора ответ" },
        { "че", "хуй через плечо" },
        { "че?", "хуй через плечо" },
    };

    public MessagePlainTextHandler(ITelegramBotClient botClient)
    {
        _botClient = botClient;
    }

    public async Task HandleAsync(Message message, CancellationToken cancellationToken)
    {
        var chatId      = message.Chat.Id;
        var messageText = message.Text;

        if (messageText is null)
            return;

        if (_messageAnswers.TryGetValue(messageText.ToLower(), out var answerText))
        {
            await _botClient.SendTextMessageAsync(chatId: chatId,
                                                  text: answerText,
                                                  replyToMessageId: message.MessageId,
                                                  cancellationToken: cancellationToken);
        }
    }
}