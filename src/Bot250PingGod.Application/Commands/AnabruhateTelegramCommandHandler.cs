using Bot250PingGod.Application.Rooms;
using Dapper;
using Infrastructure.DataAccess;
using Infrastructure.Seedwork.Providers;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Bot250PingGod.Application.Commands;

public sealed class AnabruhateTelegramCommandHandler : ITelegramCommandHandler
{
    private readonly ILogger<AnabruhateTelegramCommandHandler> _logger;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ITelegramBotClient _botClient;
    private readonly GroupMemberRepository _groupMemberRepository;
    private readonly GroupRepository _groupRepository;
    private readonly MemberRepository _memberRepository;

    public AnabruhateTelegramCommandHandler(ILogger<AnabruhateTelegramCommandHandler> logger,
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

        var username = message.From.Username ?? "username";
        var userId   = message.From.Id;

        var dbSession  = DbSession.Current;
        var connection = dbSession.Connection;

        //language=sql
        const string sql = @"
select t.id
  from bot.group_members t
 where t.is_deleted = false
   and t.chat_id != :userId
 order by random()
 limit 1;
";

        var parameters = new
        {
            userId
        };

        var commandDefinition = new CommandDefinition(sql, parameters, cancellationToken: cancellationToken);

        var randomMemberId = await connection.ExecuteScalarAsync<long>(commandDefinition);

        var chatId = command.Message.Chat.Id;

        var group  = await _groupRepository.GetByChatIdAsync(chatId, cancellationToken);
        var member = await _memberRepository.GetByChatIdAsync(chatId, cancellationToken);

        var groupMember = await _groupMemberRepository.TryGetAsync(group.Id, member.Id, cancellationToken);
        if (groupMember is null)
        {
            _logger.LogWarning("Невозможно обработать команду {TelegramCommand}, участник не зарегистрирован",
                               command.Command);

            await _botClient.SendTextMessageAsync(chatId: chatId,
                                                  text: $"Участник <i>{username}/{userId}</i> не зарегистрирован",
                                                  parseMode: ParseMode.Html,
                                                  cancellationToken: cancellationToken);

            return;
        }

        var diff = dateTime.Value - groupMember.LastAnabruhateDateTime.Value;
        if (diff.Minutes >= 30)
            groupMember.UpdateLastAnabruhateDateTime(dateTime);

        if (!groupMember.CanAnabruhate())
        {
            var limitExceededMessageText = $"{groupMember.Member.Username}, твой лимит анабрюхативаний исчерпан. " +
                                           $"Попробуй через {30 - diff.Minutes} мин";

            await _botClient.SendTextMessageAsync(chatId: chatId,
                                                  text: limitExceededMessageText,
                                                  parseMode: ParseMode.Html,
                                                  cancellationToken: cancellationToken);

            return;
        }

        groupMember.IncreaseAnabruhateCount();

        var randomMember = await _groupMemberRepository.GetAsync(randomMemberId, cancellationToken);

        _logger.LogInformation("Участник@{GroupMemberUsername}/{GroupMemberChatId} анабрюхатит @{RandomMemberUsername}",
                               message.From?.Username ?? "NoUsername", message.From?.Id, randomMember.Member.Username);

        var messageText = $"{groupMember.Member.Username} анабрюхатит: <i>{randomMember.Member.Username}</i> пошел нахуй";

        await _botClient.SendTextMessageAsync(chatId: chatId,
                                              text: messageText,
                                              parseMode: ParseMode.Html,
                                              cancellationToken: cancellationToken);

        randomMember.Anabruhate();

        await _groupMemberRepository.SaveAsync(groupMember, cancellationToken);
        await _groupMemberRepository.SaveAsync(randomMember, cancellationToken);
    }
}