using Bot250PingGod.Core.Rooms;
using Infrastructure.DataAccess;

namespace Bot250PingGod.Application.Rooms;

public class QuestionRepository
{
    public async Task<Question> GetAsync(long id, CancellationToken cancellationToken)
    {
        var nhSession = DbSession.CurrentNhSession;

        var entity = await nhSession.GetAsync<Question>(id, cancellationToken);
        if (entity is null)
        {
            throw new InvalidOperationException($"Cannot find {nameof(Question)}#{id}");
        }

        return entity;
    }

    public async Task SaveAsync(Question entity, CancellationToken cancellationToken)
    {
        var nhSession = DbSession.CurrentNhSession;

        await nhSession.SaveOrUpdateAsync(entity, cancellationToken);
        await nhSession.FlushAsync(cancellationToken);
    }
}