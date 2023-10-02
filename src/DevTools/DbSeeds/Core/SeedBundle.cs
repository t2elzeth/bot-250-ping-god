using CSharpFunctionalExtensions;
using Infrastructure.DataAccess;

namespace DbSeeds.Core;

public record TaggedEntity(string Name, Entity Entity);

public abstract class SeedBundle
{
    public IList<TaggedEntity> TaggedEntities { get; } = new List<TaggedEntity>();
    
    protected void TagEntity(string name, Entity entity)
    {
        TaggedEntities.Add(new TaggedEntity(name, entity));
    }

    protected async Task<TEntity> GetEntityAsync<TEntity>(string tag)
    {
        var id = SeedContext.GetTagId<TEntity>(tag);

        return await DbSession.CurrentNhSession.GetAsync<TEntity>(id);
    } 
}