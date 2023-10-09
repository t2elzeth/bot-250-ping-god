using Bot250PingGod.Application.Commands;
using Bot250PingGod.Application.Groups;
using Bot250PingGod.Core.Groups;
using Infrastructure.DataAccess;
using Infrastructure.Seedwork.Providers;
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
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly TelegramCommandHandler _telegramCommandHandler;
    private readonly GroupMemberRepository _groupMemberRepository;
    private readonly ISessionFactory _sessionFactory;
    private readonly GroupRepository _groupRepository;
    private readonly MemberRepository _memberRepository;

    public TelegramBot(ILogger<TelegramBot> logger,
                       ITelegramBotClient botClient,
                       IDateTimeProvider dateTimeProvider,
                       TelegramCommandHandler telegramCommandHandler,
                       GroupMemberRepository groupMemberRepository,
                       ISessionFactory sessionFactory,
                       GroupRepository groupRepository,
                       MemberRepository memberRepository)
    {
        _logger                 = logger;
        _botClient              = botClient;
        _dateTimeProvider       = dateTimeProvider;
        _telegramCommandHandler = telegramCommandHandler;
        _groupMemberRepository  = groupMemberRepository;
        _sessionFactory         = sessionFactory;
        _groupRepository        = groupRepository;
        _memberRepository       = memberRepository;
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
        var dateTime = _dateTimeProvider.Now();

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

        await using (DbSession.Bind(_sessionFactory))
        {
            await using var dbTransaction = new DbTransaction();

            var group = await _groupRepository.TryGetByChatIdAsync(message.Chat.Id, cancellationToken);
            if (group is null)
            {
                group = Group.Create(title: message.Chat.Title,
                                     chatId: message.Chat.Id);

                await _groupRepository.SaveAsync(group, cancellationToken);
            }

            var member = await _memberRepository.TryGetByChatIdAsync(message.From.Id, cancellationToken);
            if (member is null)
            {
                member = Member.Create(username: message.From.Username,
                                       firstName: message.From.FirstName,
                                       lastName: message.From.LastName,
                                       userId: message.From.Id);

                await _memberRepository.SaveAsync(member, cancellationToken);
            }

            var groupMember = await _groupMemberRepository.TryGetAsync(group.Id, member.Id, cancellationToken);
            if (groupMember is null)
            {
                groupMember = GroupMember.Create(dateTime: dateTime,
                                                 group: group,
                                                 member: member);

                await _groupMemberRepository.SaveAsync(groupMember, cancellationToken);
            }

            await dbTransaction.CommitAsync();
        }

        if (TelegramCommand.TryCreate(message, out var command))
        {
            await _telegramCommandHandler.HandleAsync(command, cancellationToken);
        }
    }
}