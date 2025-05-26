using Avalonia.Input;
using R3;

namespace Asv.Avalonia;

public sealed class HotKeyInfo : IDisposable
{
    private bool _disposed;

    public HotKeyInfo()
    {
        CustomHotKey = new ReactiveProperty<KeyGesture?>(null);
    }

    public required KeyGesture? DefaultHotKey { get; init; }
    public ReactiveProperty<KeyGesture?> CustomHotKey { get; }
    public bool IsDisposed => _disposed;

    ~HotKeyInfo()
    {
        Dispose(false);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            CustomHotKey.Dispose();
        }

        _disposed = true;
    }
}
