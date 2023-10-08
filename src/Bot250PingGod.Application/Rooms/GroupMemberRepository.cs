using Bot250PingGod.Core.Group;
using Infrastructure.DataAccess;
using NHibernate.Linq;

namespace Bot250PingGod.Application.Rooms;

public class GroupMemberRepository
{
    public async Task<GroupMember> GetAsync(long id,
                                            CancellationToken cancellationToken)
    {
        var nhSession = DbSession.CurrentNhSession;

        return await nhSession.GetAsync<GroupMember>(id, cancellationToken);
    }

    public async Task<GroupMember> GetAsync(long groupId,
                                            long memberId,
                                            CancellationToken cancellationToken)
    {
        var nhSession = DbSession.CurrentNhSession;

        var entity = await nhSession.Query<GroupMember>()
                                    .FirstOrDefaultAsync(x => x.Group.Id == groupId && x.Member.Id == memberId, cancellationToken);
        if (entity is null)
            throw new InvalidOperationException($"Cannot find {nameof(GroupMember)} with {nameof(Group.Id)}={groupId} and {nameof(Member.Id)}={memberId}");

        return entity;
    }

    public async Task<GroupMember?> TryGetAsync(long groupId,
                                                long memberId,
                                                CancellationToken cancellationToken)
    {
        var nhSession = DbSession.CurrentNhSession;

        return await nhSession.Query<GroupMember>()
                              .FirstOrDefaultAsync(x => x.Group.Id == groupId && x.Member.Id == memberId, cancellationToken);
    }

    public async Task SaveAsync(GroupMember entity, CancellationToken cancellationToken)
    {
        var nhSession = DbSession.CurrentNhSession;

        await nhSession.SaveOrUpdateAsync(entity, cancellationToken);
        await nhSession.FlushAsync(cancellationToken);
    }
}