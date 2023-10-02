using Infrastructure.DataAccess;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace Bot250PingGod.Application.Commands;

public sealed class AbruhateTelegramCommandHandler : ITelegramCommandHandler
{
    private readonly ILogger<AbruhateTelegramCommandHandler> _logger;
    private readonly ITelegramBotClient _botClient;

    public AbruhateTelegramCommandHandler(ILogger<AbruhateTelegramCommandHandler> logger,
                                          ITelegramBotClient botClient)
    {
        _logger    = logger;
        _botClient = botClient;
    }

    public async Task HandleAsync(TelegramCommand command, CancellationToken cancellationToken)
    {
        var dbSession  = DbSession.Current;
        var connection = dbSession.Connection;

        var questionText = "salam";

        var messageText = $"Вопрос: \n{questionText}";

        await _botClient.SendTextMessageAsync(chatId: command.ChatId,
                                              text: messageText,
                                              cancellationToken: cancellationToken);
    }
}