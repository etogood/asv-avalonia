using R3;

namespace Asv.Avalonia;

public class HistoricalUnitProperty : ViewModelBase
{
    private readonly ReactiveProperty<double> _model;
    private readonly IUnit _unit;
    private readonly ICommandHistory _history;
    private readonly string? _format;
    private bool _internalChange;
    private readonly IDisposable _sub1;
    private readonly IDisposable _sub2;
    private readonly IDisposable _sub3;
    private readonly IDisposable _sub4;

    public ReactiveProperty<double> Model => _model;
    public BindableReactiveProperty<string?> User { get; } = new();
    public BindableReactiveProperty<bool> IsSelected { get; } = new();
    public IUnit Unit => _unit;

    public HistoricalUnitProperty(string id, ReactiveProperty<double> model, IUnit unit, ICommandHistory history, string? format = null)
        : base(id)
    {
        _model = model;
        _unit = unit;
        _history = history;
        _format = format;
        _sub1 = _history.Register(this);
        _internalChange = true;
        _sub2 = User.EnableValidation(ValidateValue).SubscribeAwait(OnChangedByUser, AwaitOperation.Drop);
        _internalChange = false;
        _sub3 = _model.Subscribe(OnChangeByModel);
        _sub4 = unit.Current.Subscribe(_ => OnChangeByModel(model.CurrentValue));
    }

    private Exception? ValidateValue(string? userValue)
    {
        var errorMessage = _unit.Current.CurrentValue.GetValidationErrorMessage(userValue);
        return errorMessage == null ? null : new Exception(errorMessage);
    }

    private ValueTask OnChangedByUser(string? userValue, CancellationToken cancel)
    {
        if (_internalChange)
        {
            return ValueTask.CompletedTask;
        }

        var modelValue = _unit.Current.CurrentValue.ParseToSi(userValue);
        
        //return _history.Execute(new ChangePropertyCommand(), this, modelValue, cancel);
        return ValueTask.CompletedTask;
    }

    private void OnChangeByModel(double modelValue)
    {
        _internalChange = true;
        User.OnNext(_unit.Current.CurrentValue.PrintFromSi(modelValue, _format));
        _internalChange = false;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _sub1.Dispose();
            _sub2.Dispose();
            _sub3.Dispose();
            _sub4.Dispose();
            User.Dispose();
            IsSelected.Dispose();
        }
    }

}
