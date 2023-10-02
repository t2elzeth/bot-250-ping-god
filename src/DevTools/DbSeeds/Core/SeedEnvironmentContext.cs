namespace DbSeeds.Core;

public sealed class SeedEnvironmentContext : IDisposable
{
    private static string? _currentEnvironment;
    private static readonly AsyncLocal<IList<string>?> Current = new();

    public static bool CanRun
    {
        get
        {
            if (Current.Value is null)
                return true;

            if (_currentEnvironment is null)
                return false;

            return Current.Value.Any(x => x.Equals(_currentEnvironment, StringComparison.InvariantCultureIgnoreCase));
        }
    }

    public static void SetupEnvironment(string environment)
    {
        _currentEnvironment = environment;
    }

    public SeedEnvironmentContext(params string[] contexts)
    {
        Current.Value = contexts;
    }

    public void Dispose()
    {
        Current.Value = null;
    }
}