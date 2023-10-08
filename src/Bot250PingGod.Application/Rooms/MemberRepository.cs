using Bot250PingGod.Core.Group;
using Infrastructure.DataAccess;
using NHibernate.Linq;

namespace Bot250PingGod.Application.Rooms;

public sealed class MemberRepository
{
    public async Task<Member> GetAsync(long id, CancellationToken cancellationToken)
    {
        var nhSession = DbSession.CurrentNhSession;

        var entity = await nhSession.GetAsync<Member>(id, cancellationToken);
        if (entity is null)
        {
            throw new InvalidOperationException($"Cannot find {nameof(Member)}#{id}");
        }

        return entity;
    }

    public async Task<Member> GetByChatIdAsync(long chatId, CancellationToken cancellationToken)
    {
        var nhSession = DbSession.CurrentNhSession;

        var entity = await nhSession.Query<Member>()
                                    .FirstOrDefaultAsync(x => x.UserId == chatId, cancellationToken);
        if (entity is null)
            throw new InvalidOperationException($"Cannot find {nameof(Member)} with {nameof(Member.UserId)}={chatId}");

        return entity;
    }

    public async Task<Member?> TryGetByChatIdAsync(long chatId, CancellationToken cancellationToken)
    {
        var nhSession = DbSession.CurrentNhSession;

        return await nhSession.Query<Member>()
                              .FirstOrDefaultAsync(x => x.UserId == chatId, cancellationToken);
    }

    public async Task SaveAsync(Member entity, CancellationToken cancellationToken)
    {
        var nhSession = DbSession.CurrentNhSession;

        await nhSession.SaveOrUpdateAsync(entity, cancellationToken);
        await nhSession.FlushAsync(cancellationToken);
    }
}