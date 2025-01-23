using R3;

namespace Asv.Avalonia;


public class NullCommandService : ICommandService
{
    public static ICommandService Instance { get; } = new NullCommandService();
    public ICommandBase? Create(string id)
    {
        return null;
    }

    public ICommandHistory CreateHistory(string id)
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

    public ValueTask Execute(string commandId, IViewModel context, IMemento? param, CancellationToken cancel = default)
    {
        return ValueTask.CompletedTask;
    }
}