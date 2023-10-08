using Bot250PingGod.Core.Group;
using Infrastructure.DataAccess;
using NHibernate.Linq;

namespace Bot250PingGod.Application.Rooms;

public class GroupRepository
{
    public async Task<Group> GetAsync(long id, CancellationToken cancellationToken)
    {
        var nhSession = DbSession.CurrentNhSession;

        var entity = await nhSession.GetAsync<Group>(id, cancellationToken);
        if (entity is null)
        {
            throw new InvalidOperationException($"Cannot find {nameof(Group)}#{id}");
        }

        return entity;
    }

    public async Task<Group> GetByChatIdAsync(long chatId, CancellationToken cancellationToken)
    {
        var nhSession = DbSession.CurrentNhSession;

        var entity = await nhSession.Query<Group>()
                                    .FirstOrDefaultAsync(x => x.ChatId == chatId, cancellationToken);
        if (entity is null)
            throw new InvalidOperationException($"Cannot find {nameof(Group)} with {nameof(Group.ChatId)}={chatId}");

        return entity;
    }

    public async Task<Group?> TryGetByChatIdAsync(long chatId, CancellationToken cancellationToken)
    {
        var nhSession = DbSession.CurrentNhSession;

        return await nhSession.Query<Group>()
                              .FirstOrDefaultAsync(x => x.ChatId == chatId, cancellationToken);
    }

    public async Task SaveAsync(Group entity, CancellationToken cancellationToken)
    {
        var nhSession = DbSession.CurrentNhSession;

        await nhSession.SaveOrUpdateAsync(entity, cancellationToken);
        await nhSession.FlushAsync(cancellationToken);
    }
}