using Telegram.Bot;
using Telegram.Bot.Types;

namespace Bot250PingGod.Application.MessageHandlers;

public sealed class MessagePlainTextHandler
{
    private readonly ITelegramBotClient _botClient;

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

        switch (messageText.ToLower())
        {
            case "да":
            {
                await _botClient.SendTextMessageAsync(chatId: chatId,
                                                      text: "Пизда",
                                                      replyToMessageId: message.MessageId,
                                                      cancellationToken: cancellationToken);
                return;
            }

            case "нет":
            {
                await _botClient.SendTextMessageAsync(chatId: chatId,
                                                      text: "Пидора ответ",
                                                      replyToMessageId: message.MessageId,
                                                      cancellationToken: cancellationToken);
                return;
            }

            default:
                return;
        }
    }
}