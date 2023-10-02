using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;
using Humanizer;

namespace Infrastructure.DataAccess.Nh.Conventions;

public sealed class ClassConvention : IClassConvention
{
    public void Apply(IClassInstance instance)
    {
        instance.Table(instance.EntityType.Name.Underscore().Pluralize());
    }
}