using System.Reflection;
using Infrastructure.Seedwork.DataTypes;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Logging;

namespace Infrastructure.AspNetCore.Binders;

public sealed class EnumObjectBinder : IModelBinder
{
    private static Dictionary<Type, Func<string, EnumObject>> _enumFactories = new();

    private readonly ILogger<EnumObjectBinder> _logger;
    private readonly IObjectModelValidator _validator;

    public EnumObjectBinder(ILogger<EnumObjectBinder> logger,
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
            var enumFactory = GetEnumFactory(bindingContext.ModelType);

            var enumObject = enumFactory(value);

            bindingContext.Result = ModelBindingResult.Success(enumObject);

            _validator.Validate(bindingContext.ActionContext,
                                validationState: bindingContext.ValidationState,
                                prefix: string.Empty,
                                model: enumObject);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to bind '{FieldName}': {Value}", bindingContext.FieldName, value);
            bindingContext.Result = ModelBindingResult.Failed();
        }

        return Task.CompletedTask;
    }

    private static Func<string, EnumObject> GetEnumFactory(Type enumObjectType)
    {
        lock (_enumFactories)
        {
            if (!_enumFactories.TryGetValue(enumObjectType, out var factory))
            {
                var methodInfo = enumObjectType.GetMethod("Create", BindingFlags.Static | BindingFlags.Public);
                if (methodInfo is null)
                    throw new InvalidOperationException($"Cannot create EnumFactory, '{enumObjectType}' doesn't contain static 'Create' method");

                var funcType          = typeof(Func<,>);
                var factoryMethodType = funcType.MakeGenericType(typeof(string), enumObjectType);

                var enumFactory = methodInfo.CreateDelegate(factoryMethodType);

                factory = value => (EnumObject)enumFactory.DynamicInvoke(value)!;

                _enumFactories[enumObjectType] = factory;
            }

            return factory;
        }
    }
}