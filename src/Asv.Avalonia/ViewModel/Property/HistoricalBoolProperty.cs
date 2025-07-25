using Asv.Common;
using Microsoft.Extensions.Logging;
using R3;

namespace Asv.Avalonia;

public sealed class HistoricalBoolProperty : HistoricalPropertyBase<bool, bool>
{
    private readonly ReactiveProperty<bool> _modelValue;
    private bool _internalChange;
    private bool _externalChange;

    public HistoricalBoolProperty(
        NavigationId id,
        ReactiveProperty<bool> modelValue,
        ILoggerFactory loggerFactory,
        IRoutable parent
    )
        : base(id, loggerFactory, parent)
    {
        _modelValue = modelValue;
        ViewValue = new BindableReactiveProperty<bool>().DisposeItWith(Disposable);
        IsSelected = new BindableReactiveProperty<bool>().DisposeItWith(Disposable);
        ViewValue.EnableValidation(ValidateValue);

        _internalChange = true;
        ViewValue.SubscribeAwait(OnChangedByUser, AwaitOperation.Drop).DisposeItWith(Disposable);
        _internalChange = false;

        _modelValue.Subscribe(OnChangeByModel).DisposeItWith(Disposable);
    }

    protected override Exception? ValidateValue(bool userValue)
    {
        return null;
    }

    protected override async ValueTask OnChangedByUser(bool userValue, CancellationToken cancel)
    {
        if (_internalChange)
        {
            return;
        }

        _externalChange = true;
        var newValue = new BoolArg(userValue);
        await this.ExecuteCommand(ChangeBoolPropertyCommand.Id, newValue, cancel: cancel);
        _externalChange = false;
    }

    protected override void OnChangeByModel(bool modelValue)
    {
        if (_externalChange)
        {
            return;
        }

        _internalChange = true;
        ViewValue.OnNext(modelValue);
        _internalChange = false;
    }

    public override IEnumerable<IRoutable> GetRoutableChildren()
    {
        return [];
    }

    public override BindableReactiveProperty<bool> ViewValue { get; }
    public override BindableReactiveProperty<bool> IsSelected { get; }
    public override ReactiveProperty<bool> ModelValue => _modelValue;
}
