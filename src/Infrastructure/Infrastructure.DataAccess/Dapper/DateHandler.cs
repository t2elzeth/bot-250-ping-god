using System.Data;
using Dapper;
using Infrastructure.Seedwork.DataTypes;

namespace Infrastructure.DataAccess.Dapper;

public sealed class DateHandler : SqlMapper.TypeHandler<Date>
{
    public override void SetValue(IDbDataParameter parameter,
                                  Date dateTime)
    {
        parameter.Value = dateTime.Value;
    }

    public override Date Parse(object value)
    {
        return (DateTime)value;
    }
}
