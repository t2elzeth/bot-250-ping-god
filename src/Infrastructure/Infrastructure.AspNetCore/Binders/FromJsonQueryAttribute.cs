using Microsoft.AspNetCore.Mvc;

namespace Infrastructure.AspNetCore.Binders;

public class FromJsonQueryAttribute : ModelBinderAttribute
{
    public FromJsonQueryAttribute()
    {
        BinderType = typeof(JsonQueryBinder);
    }
}