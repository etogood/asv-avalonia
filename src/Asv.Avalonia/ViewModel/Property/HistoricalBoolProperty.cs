using Asv.Common;
using Microsoft.Extensions.Logging;
using R3;

namespace Asv.Avalonia;

public sealed class HistoricalBoolProperty : HistoricalPropertyBase<bool, bool>
{
    private readonly ReactiveProperty<bool> _modelValue;
    private bool _internalChange;

    public HistoricalBoolProperty(
        NavigationId id,
        ReactiveProperty<bool> modelValue,
        ILoggerFactory loggerFactory
    )
        : base(id, loggerFactory)
    {
        _modelValue = modelValue;
        ViewValue = new BindableReactiveProperty<bool>().DisposeItWith(Disposable);

        // TODO: remove async validation cause it is not needed: all validation is done at UI thread!!!
        ViewValue
            .EnableValidation(
                value =>
                {
                    var error = ValidateValue(value);
                    return error ?? ValidationResult.Success;
                },
                this,
                true,
                AwaitOperation.Drop
            )
            .DisposeItWith(Disposable);

        IsSelected = new BindableReactiveProperty<bool>().DisposeItWith(Disposable);

        _internalChange = true;

        // TODO: remove async validation cause it is not needed: all validation is done at UI thread!!!
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

        var newValue = new BoolArg(userValue);
        await this.ExecuteCommand(ChangeBoolPropertyCommand.Id, newValue, cancel: cancel);
    }

    protected override void OnChangeByModel(bool modelValue)
    {
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
