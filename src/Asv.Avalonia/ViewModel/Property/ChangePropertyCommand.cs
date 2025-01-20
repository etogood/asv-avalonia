using System.Buffers;
using MemoryPack;

namespace Asv.Avalonia;

[MemoryPackable]
public partial class ChangePropertyCommandState
{
    public string OldValue { get; set; }
    public string NewValue { get; set; }
}

public class ChangePropertyCommand : UndoableCommandBase<HistoryProperty>
{
    private ChangePropertyCommandState _state = new();

    protected override ValueTask InternalExecute(HistoryProperty context, CancellationToken cancel)
    {
        _state.OldValue = context.Model.Value;
        _state.NewValue = context.User.Value;
        context.Model.OnNext(_state.NewValue);
        return ValueTask.CompletedTask;
    }

    public override ValueTask Load(ReadOnlySequence<byte> buffer)
    {
        _state =
            MemoryPackSerializer.Deserialize<ChangePropertyCommandState>(buffer)
            ?? new ChangePropertyCommandState();
        return ValueTask.CompletedTask;
    }

    public override ValueTask Save(IBufferWriter<byte> buffer)
    {
        MemoryPackSerializer.Serialize(buffer, _state);
        return ValueTask.CompletedTask;
    }

    protected override ValueTask InternalRedo(HistoryProperty context, CancellationToken cancel)
    {
        context.IsSelected.OnNext(true);
        context.Model.OnNext(_state.NewValue);
        return ValueTask.CompletedTask;
    }

    protected override ValueTask InternalUndo(HistoryProperty context, CancellationToken cancel)
    {
        context.IsSelected.OnNext(true);
        context.Model.OnNext(_state.OldValue);
        return ValueTask.CompletedTask;
    }
}
