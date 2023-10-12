using Bot250PingGod.Application.Commands;
using Bot250PingGod.Application.MessageHandlers;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bot250PingGod.Application;

public sealed class TelegramBot
{
    private readonly ILogger<TelegramBot> _logger;
    private readonly ITelegramBotClient _botClient;
    private readonly TelegramCommandHandler _telegramCommandHandler;
    private readonly MessagePlainTextHandler _messagePlainTextHandler;

    public TelegramBot(ILogger<TelegramBot> logger,
                       ITelegramBotClient botClient,
                       TelegramCommandHandler telegramCommandHandler,
                       MessagePlainTextHandler messagePlainTextHandler)
    {
        _logger                  = logger;
        _botClient               = botClient;
        _telegramCommandHandler  = telegramCommandHandler;
        _messagePlainTextHandler = messagePlainTextHandler;
    }

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        ReceiverOptions receiverOptions = new()
        {
            AllowedUpdates      = Array.Empty<UpdateType>(),
            ThrowPendingUpdates = true
        };

        _botClient.StartReceiving(updateHandler: HandleUpdateAsync,
                                  pollingErrorHandler: HandlePollingErrorAsync,
                                  receiverOptions: receiverOptions,
                                  cancellationToken: cancellationToken);

        var me = await _botClient.GetMeAsync(cancellationToken: cancellationToken);

        _logger.LogInformation("Start listening for @{BotUsername}",
                               me.Username);

        Console.ReadLine();
    }

    private Task HandlePollingErrorAsync(ITelegramBotClient botClient,
                                         Exception exception,
                                         CancellationToken cancellationToken)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        _logger.LogError("Polling error occurred: {ErrorMessage}", errorMessage);

        return Task.CompletedTask;
    }

    private async Task HandleUpdateAsync(ITelegramBotClient botClient,
                                         Update update,
                                         CancellationToken cancellationToken)
    {
        if (update.Message is not { } message)
            return;

        if (message.Text is not { } messageText)
            return;

        if (message.Chat.Type != ChatType.Group && message.Chat.Type != ChatType.Supergroup)
        {
            _logger.LogInformation("Cannot handle message from {ChatId}, chat type is {ChatType}",
                                   message.Chat.Id, message.Chat.Type);

            return;
        }

        if (message.From is null)
        {
            _logger.LogInformation("Skipping handling update, FromUser not found");
            return;
        }

        var chatId = message.Chat.Id;

        _logger.LogInformation("Received a '{MessageText}' message in chat {ChatId}",
                               messageText, chatId);

        if (TelegramCommand.TryCreate(message, out var command))
        {
            await _telegramCommandHandler.HandleAsync(command, cancellationToken);
            return;
        }

        await _messagePlainTextHandler.HandleAsync(message, cancellationToken);
    }
}