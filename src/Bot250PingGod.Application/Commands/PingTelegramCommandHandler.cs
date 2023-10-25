using Bot250PingGod.Application.Groups;
using Infrastructure.Seedwork.Providers;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Bot250PingGod.Application.Commands;

public sealed class PingTelegramCommandHandler : ITelegramCommandHandler
{
    private readonly ILogger<PingTelegramCommandHandler> _logger;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ITelegramBotClient _botClient;
    private readonly GroupMemberRepository _groupMemberRepository;
    private readonly GroupRepository _groupRepository;
    private readonly MemberRepository _memberRepository;

    public PingTelegramCommandHandler(ILogger<PingTelegramCommandHandler> logger,
                                      IDateTimeProvider dateTimeProvider,
                                      ITelegramBotClient botClient,
                                      GroupMemberRepository groupMemberRepository,
                                      GroupRepository groupRepository,
                                      MemberRepository memberRepository)
    {
        _logger                = logger;
        _dateTimeProvider      = dateTimeProvider;
        _botClient             = botClient;
        _groupMemberRepository = groupMemberRepository;
        _groupRepository       = groupRepository;
        _memberRepository      = memberRepository;
    }

    public async Task HandleAsync(TelegramCommand command, CancellationToken cancellationToken)
    {
        var dateTime = _dateTimeProvider.Now();

        var message = command.Message;
        if (message.From is null)
        {
            _logger.LogWarning("Невозможно обработать команду {TelegramCommand}, не известный отправитель сообщения",
                               command.Command);

            return;
        }

        var chatId = command.Message.Chat.Id;

        var group         = await _groupRepository.GetByChatIdAsync(chatId, cancellationToken);
        var configuration = group.Configuration;

        if (!configuration.AllowPingCommand)
        {
            _logger.LogWarning("Cannot handle ping command, this command is disabled for Group#{ChatId}",
                               message.Chat.Id);
            return;
        }

        var member = await _memberRepository.GetByChatIdAsync(message.From.Id, cancellationToken);

        var groupMember = await _groupMemberRepository.GetAsync(group.Id, member.Id, cancellationToken);

        _logger.LogInformation("Member#{MemberId} in Group#{GroupId} with GroupMember#{GroupMemberId} wants to ping",
                               member.Id, group.Id, groupMember.Id);

        var ping = groupMember.GetPing(dateTime);

        if (!groupMember.CanPing(dateTime, out var tryAgainAfterMinutes))
        {
            _logger.LogInformation("GroupMember#{GroupMemberId} cannot ping, LastPingDateTime is {LastPingDateTime}",
                                   groupMember.Id, ping.LastPingDateTime);

            if (groupMember.ShouldNotifyPingLimit(dateTime))
            {
                var limitExceededMessageText = $"{member.Username ?? member.FirstName}, Попробуй через {tryAgainAfterMinutes} мин";

                await _botClient.SendTextMessageAsync(chatId: chatId,
                                                      text: limitExceededMessageText,
                                                      parseMode: ParseMode.Html,
                                                      cancellationToken: cancellationToken);

                groupMember.NotifyPingLimit(dateTime);
            }

            await _botClient.DeleteMessageAsync(chatId: chatId,
                                                messageId: message.MessageId,
                                                cancellationToken: cancellationToken);

            await _groupMemberRepository.SaveAsync(groupMember, cancellationToken);

            return;
        }

        var growSize = groupMember.DoPing(dateTime: dateTime);

        var messageText = $"{message.From.Username ?? message.From.FirstName}, {Math.Round(growSize, 2)} к твоему пингу. " +
                          $"Теперь твой пинг: {ping.Ping}";

        await _botClient.SendTextMessageAsync(chatId: chatId,
                                              text: messageText,
                                              parseMode: ParseMode.Html,
                                              cancellationToken: cancellationToken);

        await _groupMemberRepository.SaveAsync(groupMember, cancellationToken);
    }
}