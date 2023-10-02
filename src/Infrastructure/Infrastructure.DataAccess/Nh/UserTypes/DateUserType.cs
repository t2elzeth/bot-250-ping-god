using Infrastructure.Seedwork.DataTypes;
using NHibernate;
using NHibernate.Type;

namespace Infrastructure.DataAccess.Nh.UserTypes;

internal sealed class DateUserType : SingleValueObjectType<Date>
{
    protected override NullableType PrimitiveType => NHibernateUtil.Date;

    protected override Date Create(object value)
    {
        return Convert.ToDateTime(value);
    }

    protected override object GetValue(Date state)
    {
        return state.Value;
    }
}
