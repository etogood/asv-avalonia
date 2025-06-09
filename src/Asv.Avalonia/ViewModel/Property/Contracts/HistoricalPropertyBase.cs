using R3;

namespace Asv.Avalonia;

public abstract class HistoricalPropertyBase<TModel, TView>(NavigationId id)
    : RoutableViewModel(id),
        IHistoricalProperty<TModel>
{
    public abstract BindableReactiveProperty<TView> ViewValue { get; }
    public abstract ReactiveProperty<TModel> ModelValue { get; }
    public abstract BindableReactiveProperty<bool> IsSelected { get; }

    protected abstract Exception? ValidateValue(TView userValue);

    protected abstract ValueTask OnChangedByUser(TView userValue, CancellationToken cancel);

    protected abstract void OnChangeByModel(TModel modelValue);
}
