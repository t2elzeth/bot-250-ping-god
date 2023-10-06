using Bot250PingGod.Application.Commands;
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

    public TelegramBot(ILogger<TelegramBot> logger,
                       ITelegramBotClient botClient,
                       TelegramCommandHandler telegramCommandHandler)
    {
        _logger                 = logger;
        _botClient              = botClient;
        _telegramCommandHandler = telegramCommandHandler;
    }

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        ReceiverOptions receiverOptions = new()
        {
            AllowedUpdates      = Array.Empty<UpdateType>(),
            ThrowPendingUpdates = false
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

        var chatId = message.Chat.Id;
        
        _logger.LogInformation("Received a '{MessageText}' message in chat {ChatId}",
                               messageText, chatId);

        if (TelegramCommand.TryCreate(chatId, messageText, message, out var command))
        {
            await _telegramCommandHandler.HandleAsync(command, cancellationToken);
        }
    }
}