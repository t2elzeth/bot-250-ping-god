using CSharpFunctionalExtensions;
using Infrastructure.DataAccess;

namespace DbSeeds.Core;

public static class SeedExtensions
{
    public static async Task<long> SaveAsync<TEntity>(this TEntity entity,
                                                      string? tag = null)
        where TEntity : Entity
    {
        var nhSession = DbSession.CurrentNhSession;

        await nhSession.SaveOrUpdateAsync(entity);
        await nhSession.FlushAsync();

        if (tag is not null)
            SeedContext.PutTag<TEntity>(tag, entity.Id);

        return entity.Id;
    }

    public static async Task<long> UpdateAsync<TEntity>(this TEntity entity)
        where TEntity : Entity
    {
        var nhSession = DbSession.CurrentNhSession;

        await nhSession.UpdateAsync(entity);
        await nhSession.FlushAsync();

        return entity.Id;
    }

    public static TEntity With<TEntity>(this TEntity entity, Action<TEntity> action)
        where TEntity : Entity
    {
        action(entity);

        return entity;
    }
}