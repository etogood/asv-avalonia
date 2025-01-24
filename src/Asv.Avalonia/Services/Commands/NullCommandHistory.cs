using R3;

namespace Asv.Avalonia;


public class NullCommandService : ICommandService
{
    public static ICommandService Instance { get; } = new NullCommandService();
    public IEnumerable<ICommandMetadata> Commands => [];

    public IAsyncCommand? Create(string id)
    {
        return null;
    }

    public ICommandHistory CreateHistory(IRoutableViewModel owner)
    {
        return NullCommandHistory.Instance;
    }
}

public class NullCommandHistory : ICommandHistory
{
    public static ICommandHistory Instance { get; } = new NullCommandHistory();
    public string Id => "design";
    public IDisposable Register(IViewModel context)
    {
        return Disposable.Empty;
    }

    public void Unregister(IViewModel context)
    {
        // ignore
    }

    public IRoutableViewModel Owner { get; } = new DesignTimeShell();
    public ReactiveCommand Undo { get; } = new();
    public ValueTask UndoAsync(CancellationToken cancel = default)
    {
        return ValueTask.CompletedTask;
    }

    public ReactiveCommand Redo { get; } = new();
    public ValueTask RedoAsync(CancellationToken cancel = default)
    {
        return ValueTask.CompletedTask;
    }

    public ValueTask Execute(string commandId, IRoutableViewModel context, IPersistable? param,
        CancellationToken cancel = default)
    {
        return ValueTask.CompletedTask;
    }
}