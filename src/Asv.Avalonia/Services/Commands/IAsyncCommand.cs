using System.Buffers;

namespace Asv.Avalonia;

public interface IAsyncCommand : IStatePersistor
{
    string AsyncCommandId { get; }
    ValueTask Execute(IRoutableViewModel context, IPersistable? parameter = null, CancellationToken cancel = default);
}

/// <summary>
/// Defines an asynchronous undo-redo command interface that extends the basic
/// asynchronous command functionality with redo and undo operations.
/// </summary>
/// <remarks>
/// Implement this interface to enable asynchronous command execution
/// with the ability to persist state, and support undo and redo operations.
/// </remarks>
public interface IAsyncUndoRedoCommand : IAsyncCommand
{
    ValueTask Undo(IRoutableViewModel context, CancellationToken cancel = default);
    ValueTask Redo(IRoutableViewModel context, CancellationToken cancel = default);
}

public class ChangeStateAsyncUndoRedoCommand<TContext> : IAsyncUndoRedoCommand
    where TContext : IStatePersistor
{
    public const string CommandId = "cmd.edit.change-state";

    private IPersistable _oldState;
    private IPersistable _newState;
    public string AsyncCommandId { get; }
    public ValueTask Execute(IRoutableViewModel context, IPersistable? parameter = null, CancellationToken cancel = default)
    {
        if (context is TContext ctx && parameter != null)
        {
            _oldState = ctx.Save();
            _newState = parameter;
            ctx.Restore(parameter);
        }

        return ValueTask.CompletedTask;
    }

    public ValueTask Redo(IRoutableViewModel context, CancellationToken cancel)
    {
        if (context is TContext ctx)
        {
            ctx.Restore(_newState);
        }

        return ValueTask.CompletedTask;
    }

    public ValueTask Undo(IRoutableViewModel? context, CancellationToken cancel)
    {
        if (context is TContext ctx)
        {
            ctx.Restore(_oldState);
        }

        return ValueTask.CompletedTask;
    }

    public IPersistable Save()
    {
        throw new NotImplementedException();
    }

    public void Restore(IPersistable state)
    {
        throw new NotImplementedException();
    }
}