using Avalonia.Input;
using Material.Icons;
using R3;

namespace Asv.Avalonia;

public class NullCommandService : ICommandService
{
    public NullCommandService()
    {
        Commands = [ChangeThemeCommand.StaticInfo, UndoCommand.StaticInfo];
    }

    public static ICommandService Instance { get; } = new NullCommandService();
    public IEnumerable<ICommandInfo> Commands { get; }

    public ICommandHistory CreateHistory(IRoutable? owner)
    {
        return NullCommandHistory.Instance;
    }

    public ValueTask Execute(
        string commandId,
        IRoutable context,
        IPersistable? param = null,
        CancellationToken cancel = default
    )
    {
        return ValueTask.CompletedTask;
    }

    public KeyGesture? this[string commandId]
    {
        get => KeyGesture.Parse("Ctrl + X");
        set { }
    }

    public ValueTask Undo(CommandSnapshot command, CancellationToken cancel = default)
    {
        return ValueTask.CompletedTask;
    }

    public ValueTask Redo(CommandSnapshot command, CancellationToken cancel = default)
    {
        return ValueTask.CompletedTask;
    }

    public Observable<CommandEventArgs> OnCommand { get; } = new Subject<CommandEventArgs>();
}
