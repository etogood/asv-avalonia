using System.Buffers;
using MemoryPack;

namespace Asv.Avalonia;



[MemoryPackable]
public partial class ChangePropertyCommandState
{
    public string OldValue { get; set; }
    public string NewValue { get; set; }
}

public class ChangePropertyCommand : UndoableCommandBase<IMementoViewModel>
{
    public const string CommandId = "System.ChangeProperty";
    
    private ChangePropertyCommandState _state = new();

    protected override ValueTask InternalExecute(HistoricalUnitProperty context, CancellationToken cancel)
    {
        _state.OldValue = context.Model.Value;
        _state.NewValue = context.User.Value;
        context.Model.OnNext(_state.NewValue);
        return ValueTask.CompletedTask;
    }

    public override string Id => CommandId;

    protected override ValueTask InternalExecute(IMementoViewModel context, CancellationToken cancel)
    {
        var oldState = context.SaveState();
        
    }

    public override ValueTask Load(ReadOnlySequence<byte> buffer)
    {
        _state = MemoryPackSerializer.Deserialize<ChangePropertyCommandState>(buffer) ?? new ChangePropertyCommandState();
        return ValueTask.CompletedTask;
    }

    public override ValueTask Save(IBufferWriter<byte> buffer)
    {
        MemoryPackSerializer.Serialize(buffer, _state);
        return ValueTask.CompletedTask;
    }

    protected override ValueTask InternalRedo(IMementoViewModel context, CancellationToken cancel)
    {
        throw new NotImplementedException();
    }

    protected override ValueTask InternalUndo(IMementoViewModel context, CancellationToken cancel)
    {
        throw new NotImplementedException();
    }

    protected override ValueTask InternalRedo(HistoricalUnitProperty context, CancellationToken cancel)
    {
        context.IsSelected.OnNext(true);
        context.Model.OnNext(_state.NewValue);
        return ValueTask.CompletedTask;
    }

    protected override ValueTask InternalUndo(HistoricalUnitProperty context, CancellationToken cancel)
    {
        context.IsSelected.OnNext(true);
        context.Model.OnNext(_state.OldValue);
        return ValueTask.CompletedTask;
    }
}