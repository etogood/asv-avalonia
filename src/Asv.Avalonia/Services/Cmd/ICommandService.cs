using Avalonia.Input;
using R3;

namespace Asv.Avalonia;

public interface ICommandService
{
    IEnumerable<ICommandInfo> Commands { get; }
    ICommandHistory CreateHistory(IRoutable? owner);
    ValueTask Execute(
        string commandId,
        IRoutable context,
        IPersistable? param = null,
        CancellationToken cancel = default
    );
    KeyGesture? this[string commandId] { get; set; }
    Observable<CommandEventArgs> OnCommand { get; }
    ValueTask Undo(CommandSnapshot command, CancellationToken cancel = default);
    ValueTask Redo(CommandSnapshot command, CancellationToken cancel = default);
}

public sealed class CommandSnapshot(
    string commandId,
    string[] contextPath,
    IPersistable? parameter,
    IPersistable? undoParameter
)
{
    public string CommandId { get; set; } = commandId;
    public string[] ContextPath { get; set; } = contextPath;
    public IPersistable? Parameter { get; set; } = parameter;
    public IPersistable? UndoParameter { get; set; } = undoParameter;
}

public class CommandEventArgs(IRoutable context, ICommandFactory command, CommandSnapshot snapshot)
{
    public IRoutable Context { get; } = context;
    public ICommandFactory Command { get; } = command;
    public CommandSnapshot Snapshot { get; } = snapshot;
}
