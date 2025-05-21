using Material.Icons;

namespace Asv.Avalonia;

public interface ICommandInfo : IDisposable
{
    string Id { get; init; }
    string Name { get; init; }
    string Description { get; init; }
    MaterialIconKind Icon { get; init; }
    HotKeyInfo HotKeyInfo { get; init; }
    IExportInfo Source { get; init; }
    bool IsDisposed { get; }
}

public class CommandInfo : ICommandInfo
{
    private bool _disposed;

    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required MaterialIconKind Icon { get; init; }
    public required HotKeyInfo HotKeyInfo { get; init; }
    public required IExportInfo Source { get; init; }
    public bool IsDisposed => _disposed;

    ~CommandInfo()
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
            HotKeyInfo.Dispose();
        }

        _disposed = true;
    }
}
