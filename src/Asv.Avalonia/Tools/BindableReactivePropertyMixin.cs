using R3;

namespace Asv.Avalonia;

public static class BindableReactivePropertyMixin
{
    public static IDisposable EnableValidationRoutable<T>(
        this BindableReactiveProperty<T> prop,
        Func<T, ValidationResult> validationFunc,
        IRoutable source,
        bool isForceValidation = false
    )
    {
        prop.EnableValidation();

        if (isForceValidation)
        {
            prop.ForceValidate();
        }

        return prop.SubscribeAwait(
            async (v, _) =>
            {
                var result = validationFunc(v);
                if (result.IsFailed)
                {
                    prop.OnErrorResume(result.ValidationException);
                }

                await source.Rise(new ValidationEvent(source, prop, result));
            }
        );
    }
}
