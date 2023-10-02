using Infrastructure.Seedwork.Extensions;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Infrastructure.AspNetCore.Binders;

public class JsonModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext == null) throw new ArgumentNullException(nameof(bindingContext));

        var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
        if (valueProviderResult == ValueProviderResult.None)
            return Task.CompletedTask;

        bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);

        var valueAsString = valueProviderResult.FirstValue;
        var result        = SystemJsonSerializer.Deserialize(valueAsString, bindingContext.ModelType);

        if (result is not null)
            bindingContext.Result = ModelBindingResult.Success(result);

        return Task.CompletedTask;
    }
}