using Asv.Avalonia.Routable;
using R3;

namespace Asv.Avalonia;

public abstract class CompositeHistoricalPropertyBase<T>(Routable.NavigationId id)
    : RoutableViewModel(id),
        IHistoricalProperty<T>
{
    public abstract ReactiveProperty<T> ModelValue { get; }
}
