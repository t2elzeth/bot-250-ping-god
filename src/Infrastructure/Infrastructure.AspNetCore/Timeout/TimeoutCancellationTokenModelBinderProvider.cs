using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Infrastructure.AspNetCore.Timeout;

public class TimeoutCancellationTokenModelBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        if (context.Metadata.ModelType != typeof(CancellationToken))
            return null;

        var config = context.Services.GetRequiredService<IOptions<TimeoutOptions>>().Value;
        return new TimeoutCancellationTokenModelBinder(config);
    }

    private class TimeoutCancellationTokenModelBinder : CancellationTokenModelBinder, IModelBinder
    {
        private readonly TimeoutOptions _options;

        public TimeoutCancellationTokenModelBinder(TimeoutOptions options)
        {
            _options = options;
        }

        public new async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            await base.BindModelAsync(bindingContext);
            if (bindingContext.Result.Model is CancellationToken cancellationToken)
            {
                // combine the default token with a timeout
                var timeoutCts = new CancellationTokenSource();
                timeoutCts.CancelAfter(_options.Timeout);

                var combinedCts = CancellationTokenSource.CreateLinkedTokenSource(timeoutCts.Token, cancellationToken);

                // We need to force boxing now, so we can insert the same reference to the boxed CancellationToken
                // in both the ValidationState and ModelBindingResult.
                //
                // DO NOT simplify this code by removing the cast.
                var model = (object)combinedCts.Token;
                bindingContext.ValidationState.Clear();
                bindingContext.ValidationState.Add(model, new ValidationStateEntry { SuppressValidation = true });
                bindingContext.Result = ModelBindingResult.Success(model);
            }
        }
    }
}