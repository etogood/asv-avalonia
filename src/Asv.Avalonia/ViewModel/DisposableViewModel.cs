using R3;

namespace Asv.Avalonia;

public class DisposableViewModel(string id) : ViewModelBase(id)
{
    private volatile CancellationTokenSource? _cancel;
    private volatile CompositeDisposable? _dispose;
    private readonly object _sync = new();

    protected CancellationToken DisposeCancel
    {
        get
        {
            if (_cancel != null)
            {
                return IsDisposed ? CancellationToken.None : _cancel.Token;
            }

            lock (_sync)
            {
                if (_cancel != null)
                {
                    return IsDisposed ? CancellationToken.None : _cancel.Token;
                }

                _cancel = new CancellationTokenSource();
                return _cancel.Token;
            }
        }
    }

    protected CompositeDisposable Disposable
    {
        get
        {
            if (_dispose != null)
            {
                return _dispose;
            }

            lock (_sync)
            {
                if (_dispose != null)
                {
                    return _dispose;
                }

                var dispose = new CompositeDisposable();
                _dispose = dispose;
                return dispose;
            }
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (_cancel?.Token.CanBeCanceled == true)
            {
                _cancel.Cancel(false);
            }

            _cancel?.Dispose();
            _dispose?.Dispose();
        }
    }
}
