using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Bot250PingGod.MessageHandlers;

public sealed class StickerHandler
{
    private const string AlibekStickerFileId = "AgADOTwAAt5LIUk";

    private readonly ILogger<StickerHandler> _logger;
    private readonly ITelegramBotClient _botClient;

    public StickerHandler(ILogger<StickerHandler> logger,
                          ITelegramBotClient botClient)
    {
        _logger    = logger;
        _botClient = botClient;
    }

    public async Task HandleAsync(Message message, CancellationToken cancellationToken)
    {
        if (message.Sticker is null)
            return;

        var sticker = message.Sticker;

        Console.WriteLine(sticker.FileUniqueId);

        if (sticker.FileUniqueId == AlibekStickerFileId)
        {
            await _botClient.DeleteMessageAsync(chatId: message.Chat.Id,
                                                messageId: message.MessageId,
                                                cancellationToken: cancellationToken);

            _logger.LogInformation("Deleted Message#{MessageId}, because it is Alibek's sticker", message.MessageId);
        }
    }
}