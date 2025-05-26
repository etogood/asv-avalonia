using R3;

namespace Asv.Avalonia;

public sealed class HistoricalBoolProperty : HistoricalPropertyBase<bool, bool>
{
    private readonly ReactiveProperty<bool> _modelValue;
    private bool _internalChange;

    public HistoricalBoolProperty(NavigationId id, ReactiveProperty<bool> modelValue)
        : base(id)
    {
        _modelValue = modelValue;

        _sub1 = ViewValue.EnableValidation(
            value =>
            {
                var error = ValidateValue(value);
                return error ?? ValidationResult.Success;
            },
            this,
            true,
            AwaitOperation.Drop
        );

        _internalChange = true;
        _sub2 = ViewValue.SubscribeAwait(OnChangedByUser, AwaitOperation.Drop);
        _internalChange = false;

        _sub3 = _modelValue.Subscribe(OnChangeByModel);
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

        var newValue = new BoolCommandArg(userValue);
        await this.ExecuteCommand(ChangeBoolPropertyCommand.Id, newValue);
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

    public override BindableReactiveProperty<bool> ViewValue { get; } = new();
    public override BindableReactiveProperty<bool> IsSelected { get; } = new();
    public override ReactiveProperty<bool> ModelValue => _modelValue;

    #region Dispose

    private readonly IDisposable _sub1;
    private readonly IDisposable _sub2;
    private readonly IDisposable _sub3;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _sub1.Dispose();
            _sub2.Dispose();
            _sub3.Dispose();
            IsSelected.Dispose();
            ViewValue.Dispose();
            ModelValue.Dispose();
        }

        base.Dispose(disposing);
    }

    #endregion
}
