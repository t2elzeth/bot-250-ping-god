using Infrastructure.Seedwork.Extensions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Logging;

namespace Infrastructure.AspNetCore.Binders;

public class JsonQueryBinder : IModelBinder
{
    private readonly ILogger<JsonQueryBinder> _logger;
    private readonly IObjectModelValidator _validator;

    public JsonQueryBinder(ILogger<JsonQueryBinder> logger,
                           IObjectModelValidator validator)
    {
        _logger    = logger;
        _validator = validator;
    }

    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var value = bindingContext.ValueProvider.GetValue(bindingContext.FieldName).FirstValue;
        if (value is null)
            return Task.CompletedTask;

        try
        {
            var parsed = SystemJsonSerializer.Deserialize(value,
                                                          bindingContext.ModelType);
            bindingContext.Result = ModelBindingResult.Success(parsed);

            _validator.Validate(bindingContext.ActionContext,
                                validationState: bindingContext.ValidationState,
                                prefix: string.Empty,
                                model: parsed);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to bind '{FieldName}': {Value}", bindingContext.FieldName, value);
            bindingContext.Result = ModelBindingResult.Failed();
        }

        return Task.CompletedTask;
    }
}