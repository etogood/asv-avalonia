using Microsoft.Extensions.Logging;
using R3;

namespace Asv.Avalonia;

public abstract class CompositeHistoricalPropertyBase<T>(
    NavigationId id,
    ILoggerFactory loggerFactory
) : RoutableViewModel(id, loggerFactory), IHistoricalProperty<T>
{
    public abstract ReactiveProperty<T> ModelValue { get; }
}
