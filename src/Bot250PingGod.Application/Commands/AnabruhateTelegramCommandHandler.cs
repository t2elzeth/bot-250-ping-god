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

    public AnabruhateTelegramCommandHandler(ILogger<AnabruhateTelegramCommandHandler> logger,
                                            IDateTimeProvider dateTimeProvider,
                                            ITelegramBotClient botClient,
                                            GroupMemberRepository groupMemberRepository)
    {
        _logger                = logger;
        _dateTimeProvider      = dateTimeProvider;
        _botClient             = botClient;
        _groupMemberRepository = groupMemberRepository;
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
 order by random()
 limit 1;
";

        var randomMemberId = await connection.ExecuteScalarAsync<long>(sql, cancellationToken);

        var groupMember = await _groupMemberRepository.TryGetByChatIdAsync(message.From.Id, cancellationToken);
        if (groupMember is null)
        {
            _logger.LogWarning("Невозможно обработать команду {TelegramCommand}, участник не зарегистрирован",
                               command.Command);

            await _botClient.SendTextMessageAsync(chatId: command.ChatId,
                                                  text: $"Участник <i>{username}/{userId} не зарегистрирован",
                                                  parseMode: ParseMode.Html,
                                                  cancellationToken: cancellationToken);

            return;
        }

        var diff = dateTime.Value - groupMember.LastAnabruhateDateTime.Value;
        if (diff.Minutes >= 30)
            groupMember.UpdateLastAnabruhateDateTime(dateTime);

        if (!groupMember.CanAnabruhate())
        {
            var limitExceededMessageText = $"{groupMember.Username}, твой лимит анабрюхативаний исчерпан. " +
                                           $"Попробуй через {30 - diff.Minutes} мин";

            await _botClient.SendTextMessageAsync(chatId: command.ChatId,
                                                  text: limitExceededMessageText,
                                                  parseMode: ParseMode.Html,
                                                  cancellationToken: cancellationToken);

            return;
        }

        groupMember.IncreaseAnabruhateCount();

        await _groupMemberRepository.SaveAsync(groupMember, cancellationToken);

        var randomMember = await _groupMemberRepository.GetAsync(randomMemberId, cancellationToken);

        _logger.LogInformation("Участник@{GroupMemberUsername}/{GroupMemberChatId} анабрюхатит @{RandomMemberUsername}",
                               message.From?.Username ?? "NoUsername", message.From?.Id, randomMember.Username);

        var messageText = $"{groupMember.Username} анабрюхатит: <i>{randomMember.Username}</i> пошел нахуй";

        await _botClient.SendTextMessageAsync(chatId: command.ChatId,
                                              text: messageText,
                                              parseMode: ParseMode.Html,
                                              cancellationToken: cancellationToken);

        randomMember.Anabruhate();

        await _groupMemberRepository.SaveAsync(randomMember, cancellationToken);
    }
}