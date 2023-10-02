using Bot250PingGod.Application.Rooms;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace Bot250PingGod.Application.Commands;

public sealed class EasterEggHandler : ITelegramCommandHandler
{
    private readonly ILogger<EasterEggHandler> _logger;
    private readonly ITelegramBotClient _botClient;
    private readonly EasterEggRepository _easterEggRepository;

    public EasterEggHandler(ILogger<EasterEggHandler> logger,
                            ITelegramBotClient botClient,
                            EasterEggRepository easterEggRepository)
    {
        _logger              = logger;
        _botClient           = botClient;
        _easterEggRepository = easterEggRepository;
    }

    public async Task HandleAsync(TelegramCommand command, CancellationToken cancellationToken)
    {
        if (!command.IsEasterEgg)
        {
            _logger.LogWarning("Cannot handle {TelegramCommand}, it is not easter egg",
                               command.Command);
            return;
        }

        var easterEgg = await _easterEggRepository.SafeGetByCommandAsync(command.Command, cancellationToken);
        if (easterEgg is null)
        {
            _logger.LogWarning("Cannot handle {TelegramCommand}, Easter egg is not found",
                               command.Command);
            return;
        }

        await _botClient.SendTextMessageAsync(chatId: command.ChatId,
                                              text: easterEgg.AnswerText,
                                              cancellationToken: cancellationToken);
    }
}