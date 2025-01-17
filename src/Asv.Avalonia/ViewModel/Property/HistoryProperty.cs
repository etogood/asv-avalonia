using R3;

namespace Asv.Avalonia;

public class HistoryProperty : ViewModelBase
{
    private readonly ICommandHistory _history;
    private bool _internalChange;
    private readonly IDisposable _sub1;
    private readonly IDisposable _sub2;
    private readonly IDisposable _sub3;

    public BindableReactiveProperty<string> User { get; } = new();
    public ReactiveProperty<string> Model { get; } = new();
    public BindableReactiveProperty<bool> IsSelected { get; } = new();

    public HistoryProperty(ICommandHistory history, string id)
        : base(id)
    {
        _history = history;
        _sub1 = _history.Register(this);
        _internalChange = true;
        _sub2 = User.SubscribeAwait(OnChangedByUser, AwaitOperation.Drop);
        _internalChange = false;
        _sub3 = Model.Subscribe(OnChangeByModel);
    }

    private ValueTask OnChangedByUser(string value, CancellationToken cancel)
    {
        if (_internalChange)
        {
            return ValueTask.CompletedTask;
        }

        return _history.Execute(new ChangePropertyCommand(), this, cancel);
    }

    private void OnChangeByModel(string value)
    {
        _internalChange = true;
        User.OnNext(value);
        _internalChange = false;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _sub1.Dispose();
            _sub2.Dispose();
            _sub3.Dispose();
            User.Dispose();
            Model.Dispose();
            IsSelected.Dispose();
        }
    }
}
