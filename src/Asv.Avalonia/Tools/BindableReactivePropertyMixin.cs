using System.Linq.Expressions;
using R3;

namespace Asv.Avalonia;

public static class BindableReactivePropertyMixin
{
    public static IDisposable EnableValidation<T>(
        this BindableReactiveProperty<T> prop,
        Func<T, ValueTask<ValidationResult>> validationFunc,
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
                var result = await validationFunc(v);
                if (result.IsFailed)
                {
                    prop.OnErrorResume(result.ValidationException);
                }

                await source.Rise(new ValidationEvent(source, prop, result));
            }
        );
    }
}
