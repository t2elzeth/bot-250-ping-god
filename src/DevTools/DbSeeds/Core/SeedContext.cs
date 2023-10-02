namespace DbSeeds.Core;

public sealed class SeedContext : IDisposable
{
    private static readonly AsyncLocal<Dictionary<string, long>?> Current = new();

    public static void PutTag(string tag, long value)
    {
        var current = Current.Value ?? throw new InvalidOperationException("No seed context");

        current[tag] = value;
    }
    
    public static void PutTag<TEntity>(string tag, long value)
    {
        var current = Current.Value ?? throw new InvalidOperationException("No seed context");

        var typedTag = GetTypedTag<TEntity>(tag);

        current[typedTag] = value;
    }

    public static long GetTagId<TEntity>(string tag)
    {
        var current = Current.Value ?? throw new InvalidOperationException("No seed context");

        var typedTag = GetTypedTag<TEntity>(tag);

        return current[typedTag];
    }

    public SeedContext()
    {
        Current.Value = new Dictionary<string, long>();
    }

    public void Dispose()
    {
        Current.Value = null;
    }
    
    private static string GetTypedTag<TEntity>(string key)
    {
        var entityName = typeof(TEntity).Name;

        return $"{entityName}[{key}]";
    }
}