namespace DbSeeds.Core;

[AttributeUsage(AttributeTargets.Method)]
public sealed class MigrationSeedAttribute : Attribute
{
    private readonly string[] _contexts;

    public MigrationSeedAttribute(params string[] contexts)
    {
        _contexts = contexts;
    }

    public bool CanRun(string environment)
    {
        if (_contexts.Length == 0)
            return true;

        return _contexts.Any(x => x.Equals(environment, StringComparison.InvariantCultureIgnoreCase));
    }
}