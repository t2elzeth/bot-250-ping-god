using System.Data;
using Bot250PingGod.Application.Rooms;
using Dapper;
using Infrastructure.DataAccess;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace Bot250PingGod.Application.Commands;

public sealed class AskTelegramCommandHandler : ITelegramCommandHandler
{
    private readonly ILogger<AskTelegramCommandHandler> _logger;
    private readonly ITelegramBotClient _botClient;
    private readonly QuestionRepository _questionRepository;

    public AskTelegramCommandHandler(ILogger<AskTelegramCommandHandler> logger,
                                     ITelegramBotClient botClient,
                                     QuestionRepository questionRepository)
    {
        _logger             = logger;
        _botClient          = botClient;
        _questionRepository = questionRepository;
    }

    public async Task HandleAsync(TelegramCommand command, CancellationToken cancellationToken)
    {
        var dbSession  = DbSession.Current;
        var connection = dbSession.Connection;

        var questionIds = await QueryQuestionIds(connection, cancellationToken);
        if (questionIds.Count == 0)
        {
            _logger.LogWarning("Список вопросов пустой");
            await _botClient.SendTextMessageAsync(chatId: command.ChatId,
                                                  text: "Список вопросов пуст.\nПожалуйста, обратитесь к моему бате @garbageoff",
                                                  cancellationToken: cancellationToken);
            return;
        }

        var random           = new Random();
        var randomQuestionId = random.Next(1, questionIds.Count + 1);

        var question = await _questionRepository.GetAsync(randomQuestionId, cancellationToken);

        var questionText = question.Text;

        var messageText = $"Вопрос: \n{questionText}";

        await _botClient.SendTextMessageAsync(chatId: command.ChatId,
                                              text: messageText,
                                              cancellationToken: cancellationToken);
    }

    private static async Task<IList<long>> QueryQuestionIds(IDbConnection connection,
                                                            CancellationToken cancellationToken)
    {
        //language=sql
        const string sql = @"
select t.id
  from public.questions t
";

        var result = await connection.QueryAsync<long>(sql, cancellationToken);

        return result.ToList();
    }
}