using Bot250PingGod.Core.Abruhate;
using Infrastructure.DataAccess;
using NHibernate.Linq;

namespace Bot250PingGod.Application.Rooms;

public class GroupMemberRepository
{
    public async Task<GroupMember> GetAsync(long id, CancellationToken cancellationToken)
    {
        var nhSession = DbSession.CurrentNhSession;

        var entity = await nhSession.GetAsync<GroupMember>(id, cancellationToken);
        if (entity is null)
        {
            throw new InvalidOperationException($"Cannot find {nameof(GroupMember)}#{id}");
        }

        return entity;
    }

    public async Task<GroupMember?> TryGetByUsernameAsync(string username, CancellationToken cancellationToken)
    {
        var nhSession = DbSession.CurrentNhSession;

        return await nhSession.Query<GroupMember>()
                              .FirstOrDefaultAsync(x => x.Username == username, cancellationToken);
    }

    public async Task<GroupMember?> TryGetByChatIdAsync(long chatId, CancellationToken cancellationToken)
    {
        var nhSession = DbSession.CurrentNhSession;

        return await nhSession.Query<GroupMember>()
                              .FirstOrDefaultAsync(x => x.ChatId == chatId, cancellationToken);
    }

    public async Task SaveAsync(GroupMember entity, CancellationToken cancellationToken)
    {
        var nhSession = DbSession.CurrentNhSession;

        await nhSession.SaveOrUpdateAsync(entity, cancellationToken);
        await nhSession.FlushAsync(cancellationToken);
    }
}