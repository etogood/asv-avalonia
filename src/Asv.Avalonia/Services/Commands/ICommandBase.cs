using System.Buffers;
using System.Windows.Input;

namespace Asv.Avalonia;

public interface ICommandBase
{
    string Id { get; }
    ValueTask Execute(object context, object? parameter = null, CancellationToken cancel = default);
    ValueTask Load(ReadOnlySequence<byte> buffer);
    ValueTask Save(IBufferWriter<byte> buffer);
}

public interface IUndoableCommand : ICommandBase
{
    ValueTask Redo(object context, CancellationToken cancel);
    ValueTask Undo(object context, CancellationToken cancel);
}

public abstract class UndoableCommandBase<TContext> : IUndoableCommand
{
    public abstract string Id { get; }

    public ValueTask Execute(object context, CancellationToken cancel)
    {
        if (context is TContext ctx)
        {
            return InternalExecute(ctx, cancel);
        }

        return ValueTask.CompletedTask;
    }

    protected abstract ValueTask InternalExecute(TContext context, CancellationToken cancel);

    public abstract ValueTask Load(ReadOnlySequence<byte> buffer);

    public abstract ValueTask Save(IBufferWriter<byte> buffer);

    public ValueTask Redo(object context, CancellationToken cancel)
    {
        if (context is TContext ctx)
        {
            return InternalRedo(ctx, cancel);
        }

        return ValueTask.CompletedTask;
    }
    
    protected abstract ValueTask InternalRedo(TContext context, CancellationToken cancel);
    public ValueTask Undo(object context, CancellationToken cancel)
    {
        if (context is TContext ctx)
        {
            return InternalUndo(ctx, cancel);
        }

        return ValueTask.CompletedTask;
    }

    protected abstract ValueTask InternalUndo(TContext context, CancellationToken cancel);
}