using Microsoft.AspNetCore.Mvc;

namespace Infrastructure.AspNetCore.Binders;

[AttributeUsage(AttributeTargets.Property)]
public class FromJsonAttribute : ModelBinderAttribute
{
    public FromJsonAttribute()
    {
        BinderType = typeof(JsonModelBinder);
    }
}