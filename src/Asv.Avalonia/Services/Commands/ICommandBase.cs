using System.Buffers;

namespace Asv.Avalonia;

public interface ICommandBase : IOriginator
{
    string Id { get; }
    ValueTask Execute(object? context, IMemento? parameter = null, CancellationToken cancel = default);
}

public interface IUndoableCommand : ICommandBase
{
    ValueTask Undo(object? context, CancellationToken cancel = default);
    ValueTask Redo(object? context, CancellationToken cancel = default);
}

public class ChangeStateCommand<TContext> : IUndoableCommand
    where TContext : IOriginator
{
    public const string CommandId = "cmd.edit.change-state";
    
    private IMemento _oldState;
    private IMemento _newState;
    public string Id { get; }
    public ValueTask Execute(object? context, IMemento? parameter = null, CancellationToken cancel = default)
    {
        if (context is TContext ctx && parameter != null)
        {
            _oldState = ctx.Save();
            _newState = parameter;
            ctx.Restore(parameter);
        }

        return ValueTask.CompletedTask;
    }

    public ValueTask Redo(object? context, CancellationToken cancel)
    {
        if (context is TContext ctx)
        {
            ctx.Restore(_newState);
        }

        return ValueTask.CompletedTask;
    }

    public ValueTask Undo(object? context, CancellationToken cancel)
    {
        if (context is TContext ctx)
        {
            ctx.Restore(_oldState);
        }

        return ValueTask.CompletedTask;
    }

    public IMemento Save()
    {
        throw new NotImplementedException();
    }

    public void Restore(IMemento state)
    {
        throw new NotImplementedException();
    }
}