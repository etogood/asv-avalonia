using Asv.Avalonia.Routable;
using Avalonia.Input;
using R3;

namespace Asv.Avalonia;

public interface ICommandService : IExportable
{
    IEnumerable<ICommandInfo> Commands { get; }
    ICommandHistory CreateHistory(IRoutable? owner);
    ValueTask Execute(
        string commandId,
        IRoutable context,
        CommandArg param,
        CancellationToken cancel = default
    );
    void SetHotKey(string commandId, HotKeyInfo hotKey);
    HotKeyInfo GetHotKey(string commandId);
    Observable<CommandSnapshot> OnCommand { get; }
    ValueTask Undo(CommandSnapshot command, CancellationToken cancel = default);
    ValueTask Redo(CommandSnapshot command, CancellationToken cancel = default);
}
