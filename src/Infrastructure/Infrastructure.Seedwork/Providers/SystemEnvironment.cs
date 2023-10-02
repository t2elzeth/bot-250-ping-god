namespace Infrastructure.Seedwork.Providers;

public sealed class SystemEnvironmentConfiguration
{
    public string SystemTimeZone { get; set; } = null!;

    public bool Standby { get; set; }
}

public sealed class SystemEnvironment
{
    public TimeZoneInfo SystemTimeZone { get; }

    public bool Standby { get; }

    public SystemEnvironment(SystemEnvironmentConfiguration configuration)
    {
        SystemTimeZone = TimeZoneInfo.FindSystemTimeZoneById(configuration.SystemTimeZone);
        Standby        = configuration.Standby;
    }

    public SystemEnvironment(TimeZoneInfo systemTimeZone, bool standby)
    {
        SystemTimeZone = systemTimeZone;
        Standby        = standby;
    }
}