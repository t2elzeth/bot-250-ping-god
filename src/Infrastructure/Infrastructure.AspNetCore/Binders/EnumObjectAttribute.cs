using Microsoft.AspNetCore.Mvc;

namespace Infrastructure.AspNetCore.Binders;

[AttributeUsage(AttributeTargets.Property)]
public class EnumObjectAttribute : ModelBinderAttribute
{
    public EnumObjectAttribute()
    {
        BinderType = typeof(EnumObjectBinder);
    }
}