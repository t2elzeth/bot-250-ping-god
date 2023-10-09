using Bot250PingGod.Application.Groups;
using Infrastructure.Seedwork.Providers;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Bot250PingGod.Application.Commands;

public sealed class GrowPussyTelegramCommandHandler : ITelegramCommandHandler
{
    private readonly ILogger<GrowPussyTelegramCommandHandler> _logger;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ITelegramBotClient _botClient;
    private readonly GroupMemberRepository _groupMemberRepository;
    private readonly GroupRepository _groupRepository;
    private readonly MemberRepository _memberRepository;

    public GrowPussyTelegramCommandHandler(ILogger<GrowPussyTelegramCommandHandler> logger,
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

        var group  = await _groupRepository.GetByChatIdAsync(chatId, cancellationToken);
        var member = await _memberRepository.GetByChatIdAsync(message.From.Id, cancellationToken);

        var groupMember = await _groupMemberRepository.GetAsync(group.Id, member.Id, cancellationToken);

        _logger.LogInformation("Member#{MemberId} in Group#{GroupId} with GroupMember#{GroupMemberId} wants to grow pussy",
                               member.Id, group.Id, groupMember.Id);

        var pussy = groupMember.Pussy;

        if (!pussy.CanGrow(dateTime, out var tryAgainAfterMinutes))
        {
            _logger.LogInformation("GroupMember#{GroupMemberId} cannot grow pussy, LastPussyGrowDateTime is {LastPussyGrowDateTime}",
                                   groupMember.Id, pussy.LastGrowDateTime);

            if (pussy.ShouldNotifyLimit(dateTime))
            {
                var limitExceededMessageText = $"{member.Username ?? member.FirstName}, Попробуй через {tryAgainAfterMinutes} мин";

                await _botClient.SendTextMessageAsync(chatId: chatId,
                                                      text: limitExceededMessageText,
                                                      parseMode: ParseMode.Html,
                                                      cancellationToken: cancellationToken);

                pussy.NotifyLimit(dateTime);
            }

            await _botClient.DeleteMessageAsync(chatId: chatId,
                                                messageId: message.MessageId,
                                                cancellationToken: cancellationToken);

            await _groupMemberRepository.SaveAsync(groupMember, cancellationToken);

            return;
        }

        var growSize = pussy.Grow(dateTime);

        var messageText = $"{message.From.Username ?? message.From.FirstName}, {Math.Round(growSize, 2)} см. к глубине твоей пусси. " +
                          $"Теперь ее глубина: {pussy.Size} см";

        await _botClient.SendTextMessageAsync(chatId: chatId,
                                              text: messageText,
                                              parseMode: ParseMode.Html,
                                              cancellationToken: cancellationToken);

        await _groupMemberRepository.SaveAsync(groupMember, cancellationToken);
    }
}