using Infrastructure.Seedwork.DataTypes;
using NHibernate;
using NHibernate.Type;

namespace Infrastructure.DataAccess.Nh.UserTypes;

internal sealed class UtcDateTimeUserType : SingleValueObjectType<UtcDateTime>
{
    protected override NullableType PrimitiveType => NHibernateUtil.UtcDateTime;

    protected override UtcDateTime Create(object value)
    {
        return Convert.ToDateTime(value);
    }

    protected override object GetValue(UtcDateTime state)
    {
        return state.Value;
    }
}