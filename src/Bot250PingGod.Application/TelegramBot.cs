using Bot250PingGod.Application.Commands;
using Bot250PingGod.Application.Rooms;
using Infrastructure.DataAccess;
using Microsoft.Extensions.Logging;
using NHibernate;
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
    private readonly GroupMemberRepository _groupMemberRepository;
    private readonly ISessionFactory _sessionFactory;

    public TelegramBot(ILogger<TelegramBot> logger,
                       ITelegramBotClient botClient,
                       TelegramCommandHandler telegramCommandHandler,
                       GroupMemberRepository groupMemberRepository,
                       ISessionFactory sessionFactory)
    {
        _logger                 = logger;
        _botClient              = botClient;
        _telegramCommandHandler = telegramCommandHandler;
        _groupMemberRepository  = groupMemberRepository;
        _sessionFactory         = sessionFactory;
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

        if (message.From is not null)
        {
            var username = message.From.Username;

            if (username is not null)
            {
                await using (DbSession.Bind(_sessionFactory))
                {
                    await using var dbTransaction = new DbTransaction();

                    var groupMember = await _groupMemberRepository.TryGetByUsernameAsync(username, cancellationToken);

                    if (groupMember is not null)
                    {
                        groupMember.UpdateChatId(message.From.Id);

                        await _groupMemberRepository.SaveAsync(groupMember, cancellationToken);
                    }

                    await dbTransaction.CommitAsync();
                }
            }
        }

        if (TelegramCommand.TryCreate(chatId, messageText, message, out var command))
        {
            await _telegramCommandHandler.HandleAsync(command, cancellationToken);
        }
    }
}