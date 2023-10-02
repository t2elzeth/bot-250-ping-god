using System.Globalization;
using CSharpFunctionalExtensions;
using Infrastructure.Seedwork.Validation;

namespace Infrastructure.Seedwork.DataTypes;

public class UtcDateTime : SingleValueObject<DateTime>, IComparable<UtcDateTime>
{
    private const string Format = "yyyy-MM-ddTHH:mm:ssZ";

    private UtcDateTime(DateTime value)
        : base(value)
    {
    }

    public override string ToString()
    {
        return Value.ToString(Format);
    }

    public Date Today(TimeZoneInfo timeZone)
    {
        var tzNow = TimeZoneInfo.ConvertTimeFromUtc(Value, timeZone);
        return new Date(tzNow.Year, tzNow.Month, tzNow.Day);
    }
    
    public int CompareTo(UtcDateTime? other)
    {
        return other is null 
            ? 1 
            : DateTime.Compare(Value, other.Value);
    }

    public static implicit operator DateTime(UtcDateTime value)
    {
        return value.Value;
    }

    public static explicit operator string(UtcDateTime value)
    {
        return value.Value.ToString(Format);
    }

    public static explicit operator UtcDateTime(string value)
    {
        var result = Create(value);

        if (result.IsFailure)
            throw new InvalidOperationException(result.Error.Message);

        return result.Value;
    }

    public static implicit operator UtcDateTime(DateTime dateTime)
    {
        if (dateTime.Kind == DateTimeKind.Unspecified)
            throw new InvalidOperationException($"Wrong DateTime {dateTime:yyyy.MM.dd HH:mm:ss fffffff}, DateTime.Kind is unspecified");

        var universalTime = dateTime.ToUniversalTime();

        return new UtcDateTime(universalTime);
    }

    public static bool operator >(UtcDateTime x, UtcDateTime y)
    {
        return x.Value > y.Value;
    }
    
    public static bool operator >=(UtcDateTime x, UtcDateTime y)
    {
        return x.Value >= y.Value;
    }
    
    public static bool operator <(UtcDateTime x, UtcDateTime y)
    {
        return x.Value > y.Value;
    }
    
    public static bool operator <=(UtcDateTime x, UtcDateTime y)
    {
        return x.Value >= y.Value;
    }

    public static UtcDateTime operator +(UtcDateTime utcDateTime, TimeSpan timeSpan)
    {
        return utcDateTime.Value + timeSpan;
    }

    public static UtcDateTime operator -(UtcDateTime utcDateTime, TimeSpan timeSpan)
    {
        return utcDateTime.Value - timeSpan;
    }

    public static Result<UtcDateTime, SystemError> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return SystemError.FillFieldMessage;

        if (!DateTime.TryParse(value,
                               CultureInfo.InvariantCulture,
                               DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeLocal,
                               out var dateTime))
        {
            return SystemError.WrongFormat;
        }

        return new UtcDateTime(dateTime);
    }
}