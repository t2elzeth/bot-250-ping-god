using CSharpFunctionalExtensions;
using FluentValidation;

namespace Infrastructure.Seedwork.Validation;

public static class CustomValidators
{
    public static IRuleBuilderOptions<T, TProperty> MustBeValueObject<T, TValueObject, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder,
                                                                                                  Func<TProperty, Result<TValueObject, SystemError>> factoryMethod)
    {
        return (IRuleBuilderOptions<T, TProperty>)ruleBuilder.Custom((value, context) =>
        {
            var result = factoryMethod(value!);

            if (result.IsFailure)
                context.AddFailure(result.Error.Message);
        });
    }
}