using Microsoft.Extensions.Logging;
using R3;

namespace Asv.Avalonia;

public abstract class CompositeHistoricalPropertyBase<T> : RoutableViewModel, IHistoricalProperty<T>
{
    protected CompositeHistoricalPropertyBase(
        NavigationId id,
        ILoggerFactory loggerFactory,
        IRoutable parent
    )
        : base(id, loggerFactory)
    {
        Parent = parent;
    }

    public abstract ReactiveProperty<T> ModelValue { get; }
    public abstract void ForceValidate();
}
