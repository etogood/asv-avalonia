using System.Composition;

namespace Asv.Avalonia;

[Export(typeof(ICommandFactory))]
[Shared]
public class ChangeThemeCommandFactory : ICommandFactory
{
    private readonly IThemeService _svc;
    private readonly IShellHost _host;

    [ImportingConstructor]
    public ChangeThemeCommandFactory(IThemeService svc, IShellHost host)
    {
        ArgumentNullException.ThrowIfNull(svc);
        _svc = svc;
        _host = host;
    }

    public string CommandId => ChangeThemeAsyncUndoRedoCommand.CommandId;
    public string Name => "Change theme";
    public string Description => "Change application theme";
    public string Icon { get; } = string.Empty;
    public int Order => 0;

    public IAsyncCommand Create()
    {
        return new ChangeThemeAsyncUndoRedoCommand(_svc);
    }
}

public class ChangeThemeAsyncUndoRedoCommand(IThemeService svc) : IAsyncUndoRedoCommand
{
    private PersistableChange<string>? _state;
    public const string CommandId = "theme.change";
    public IPersistable Save()
    {
        return _state ?? throw new InvalidOperationException();
    }

    public void Restore(IPersistable state)
    {
        if (state is PersistableChange<string> memento)
        {
            _state = memento;
        }
    }

    public string AsyncCommandId => CommandId;

    public ValueTask Execute(IRoutableViewModel context, IPersistable? parameter = null, CancellationToken cancel = default)
    {
        if (parameter is Persistable<string> memento)
        {
            var oldValue = svc.CurrentTheme.Value.Id;
            var theme = svc.Themes.FirstOrDefault(x => x.Id == memento.Value);
            if (theme != null)
            {
                svc.CurrentTheme.Value = theme;
            }

            _state = new PersistableChange<string>(oldValue, memento.Value);
        }

        return ValueTask.CompletedTask;
    }

    public ValueTask Undo(IRoutableViewModel? context, CancellationToken cancel = default)
    {
        if (_state != null)
        {
            var theme = svc.Themes.FirstOrDefault(x => x.Id == _state.OldValue);
            if (theme != null)
            {
                svc.CurrentTheme.Value = theme;
            }
        }

        return ValueTask.CompletedTask;
    }

    public ValueTask Redo(IRoutableViewModel context, CancellationToken cancel = default)
    {
        if (_state != null)
        {
            var theme = svc.Themes.FirstOrDefault(x => x.Id == _state.NewValue);
            if (theme != null)
            {
                svc.CurrentTheme.Value = theme;
            }
        }

        return ValueTask.CompletedTask;
    }
}