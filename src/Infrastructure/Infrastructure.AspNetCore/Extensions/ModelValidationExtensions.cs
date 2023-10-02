using Infrastructure.Seedwork.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.AspNetCore.Extensions;

public static class ModelValidationExtensions
{
    public static IMvcCoreBuilder UseCustomModelValidation(this IMvcCoreBuilder builder)
    {
        return UseCustomModelValidation(builder, () => new SystemError());
    }

    public static IMvcCoreBuilder UseCustomModelValidation(this IMvcCoreBuilder builder, Func<SystemError> errorFactory)
    {
        return builder.ConfigureApiBehaviorOptions(options => //
        {
            options.InvalidModelStateResponseFactory = context => CustomErrorResponse(context, errorFactory);
        });
    }

    private static BadRequestObjectResult CustomErrorResponse(ActionContext actionContext, Func<SystemError> errorFactory)
    {
        var errorResult = errorFactory();

        var modelState = actionContext.ModelState;

        if (modelState.Root.Errors.Count > 0)
        {
            var rootError = modelState.Root.Errors.First();
            errorResult.Message = rootError.ErrorMessage;
        }

        var errorEntries = modelState.Where(modelError => modelError.Value!.Errors.Count > 0);

        foreach (var (parameterName, errorEntry) in errorEntries)
        {
            if (errorEntry!.Errors.Count == 0)
                continue;

            var parameterError = errorEntry.Errors.First();
            errorResult.SetError(parameterName, parameterError.ErrorMessage);
        }

        return new BadRequestObjectResult(errorResult);
    }
}