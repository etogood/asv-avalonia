using R3;

namespace Asv.Avalonia;

public abstract class CompositeHistoricalPropertyBase<T>(NavigationId id)
    : RoutableViewModel(id),
        IHistoricalProperty<T>
{
    public abstract ReactiveProperty<T> ModelValue { get; }
}
