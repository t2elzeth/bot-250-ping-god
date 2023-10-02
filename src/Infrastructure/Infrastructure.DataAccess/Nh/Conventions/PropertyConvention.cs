using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;
using Humanizer;

namespace Infrastructure.DataAccess.Nh.Conventions;

public sealed class PropertyConvention : IPropertyConvention
{
    public void Apply(IPropertyInstance instance)
    {
        instance.Column(instance.Name.Underscore());
    }
}