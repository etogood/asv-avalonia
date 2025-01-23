using System.Reflection.Metadata.Ecma335;
using R3;

namespace Asv.Avalonia;

public class HistoricalUnitProperty : RoutableViewModel, IOriginator
{
    private readonly ReactiveProperty<double> _model;
    private readonly IUnit _unit;
    private readonly string? _format;
    private bool _internalChange;
    private readonly IDisposable _sub2;
    private readonly IDisposable _sub3;
    private readonly IDisposable _sub4;

    public ReactiveProperty<double> Model => _model;
    public BindableReactiveProperty<string?> User { get; } = new();
    public BindableReactiveProperty<bool> IsSelected { get; } = new();
    public IUnit Unit => _unit;

    public HistoricalUnitProperty(string id, ReactiveProperty<double> model, IUnit unit, string? format = null)
        : base(id)
    {
        _model = model;
        _unit = unit;
        _format = format;
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

        var newValue = new Memento<double>(_unit.Current.CurrentValue.ParseToSi(userValue));
        return this.ExecuteCommand(ChangeStateCommand<IOriginator>.CommandId, newValue);
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
            _sub2.Dispose();
            _sub3.Dispose();
            _sub4.Dispose();
            User.Dispose();
            IsSelected.Dispose();
        }
    }

    public override IEnumerable<IRoutableViewModel> Children => ArraySegment<IRoutableViewModel>.Empty;
    protected override ValueTask InternalCatchEvent(AsyncRoutedEvent e)
    {
        return ValueTask.CompletedTask;
    }

    public IMemento Save()
    {
        return new Memento<double>(Model.Value);
    }

    public void Restore(IMemento state)
    {
        if (state is Memento<double> value)
        {
            Model.OnNext(value.Value);
        }
    }
}
