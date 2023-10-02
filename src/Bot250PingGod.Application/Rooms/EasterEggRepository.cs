using Bot250PingGod.Core.Rooms;
using Infrastructure.DataAccess;
using NHibernate.Linq;

namespace Bot250PingGod.Application.Rooms;

public class EasterEggRepository
{
    public async Task<EasterEgg> GetAsync(long id, CancellationToken cancellationToken)
    {
        var nhSession = DbSession.CurrentNhSession;

        var entity = await nhSession.GetAsync<EasterEgg>(id, cancellationToken);
        if (entity is null)
        {
            throw new InvalidOperationException($"Cannot find {nameof(EasterEgg)}#{id}");
        }

        return entity;
    }

    public async Task<EasterEgg?> SafeGetByCommandAsync(string commandText, CancellationToken cancellationToken)
    {
        var nhSession = DbSession.CurrentNhSession;

        return await nhSession.Query<EasterEgg>()
                              .Where(x => x.Command == commandText)
                              .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task SaveAsync(EasterEgg entity, CancellationToken cancellationToken)
    {
        var nhSession = DbSession.CurrentNhSession;

        await nhSession.SaveOrUpdateAsync(entity, cancellationToken);
        await nhSession.FlushAsync(cancellationToken);
    }
}