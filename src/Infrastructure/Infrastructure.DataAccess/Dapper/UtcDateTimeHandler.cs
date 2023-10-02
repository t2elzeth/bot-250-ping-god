using System.Data;
using Dapper;
using Infrastructure.Seedwork.DataTypes;

namespace Infrastructure.DataAccess.Dapper;

public class UtcDateTimeHandler : SqlMapper.TypeHandler<UtcDateTime>
{
    public override void SetValue(IDbDataParameter parameter,
                                  UtcDateTime dateTime)
    {
        parameter.Value = dateTime.Value;
    }

    public override UtcDateTime Parse(object value)
    {
        var dateTime = (DateTime)value;

        return new DateTime(dateTime.Year,
                            dateTime.Month,
                            dateTime.Day,
                            dateTime.Hour,
                            dateTime.Minute,
                            dateTime.Second,
                            dateTime.Millisecond,
                            DateTimeKind.Utc);
    }
}
