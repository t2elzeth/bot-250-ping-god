using Infrastructure.Seedwork.DataTypes;
using JetBrains.Annotations;

namespace Infrastructure.Seedwork.Providers;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public interface IDateTimeProvider
{
    UtcDateTime Now();

    Date Today();
}

public class DateTimeProvider : IDateTimeProvider
{
    private readonly SystemEnvironment _systemEnvironment;

    public DateTimeProvider(SystemEnvironment systemEnvironment)
    {
        _systemEnvironment = systemEnvironment;
    }

    public UtcDateTime Now()
    {
        return DateTime.UtcNow;
    }

    public Date Today()
    {
        return Now().Today(_systemEnvironment.SystemTimeZone);
    }
}