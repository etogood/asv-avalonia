using System.Collections.Immutable;
using System.Composition;
using Avalonia.Input;
using Material.Icons;

namespace Asv.Avalonia;

[ExportCommand]
[Shared]
public class ChangeThemeCommandFactory : ICommandFactory
{
    private readonly IThemeService _svc;

    [ImportingConstructor]
    public ChangeThemeCommandFactory(IThemeService svc)
    {
        ArgumentNullException.ThrowIfNull(svc);
        _svc = svc;
    }

    public ICommandInfo Info => ChangeThemeCommand.StaticInfo;

    public IAsyncCommand Create(IRoutable context, IPersistable? parameter = null)
    {
        return new ChangeThemeCommand(_svc, context, parameter);
    }

    public bool CanExecute(IRoutable context, IPersistable? parameter)
    {
        target = context;
        return true;
    }
}

public class ChangeThemeCommand : IUndoRedoCommand
{
    public const string Id = "theme.change";
    internal static readonly ICommandInfo StaticInfo = new CommandInfo
    {
        Id = Id,
        Name = RS.ChangeThemeCommand_CommandInfo_Name,
        Description = RS.ChangeThemeCommand_CommandInfo_Description,
        Icon = MaterialIconKind.ThemeLightDark,
        DefaultHotKey = KeyGesture.Parse("Ctrl+T"),
        Order = 0,
    };

    private readonly IThemeService _svc;
    private readonly IRoutable _context;
    private readonly IPersistable? _parameter;
    private PersistableChange<string>? _state;

    public ChangeThemeCommand(IThemeService svc, IRoutable context, IPersistable? parameter)
    {
        _svc = svc;
        _context = context;
        _parameter = parameter;
    }

    public ICommandInfo Info => StaticInfo;

    public ValueTask Execute(CancellationToken cancel = default)
    {
        if (_parameter is Persistable<string> memento)
        {
            // execute with parameter
            var oldValue = _svc.CurrentTheme.Value.Id;
            var theme = _svc.Themes.FirstOrDefault(x => x.Id == memento.Value);
            if (theme != null)
            {
                _svc.CurrentTheme.Value = theme;
            }

            _state = new PersistableChange<string>(oldValue, memento.Value);
        }
        else
        {
            // execute without parameter
            var oldValue = _svc.CurrentTheme.Value.Id;
            var temp = _svc.Themes.ToList();
            var index = temp.IndexOf(_svc.CurrentTheme.Value);
            index++;
            if (index >= temp.Count)
            {
                index = 0;
            }

            var newValue = temp[index].Id;
            _svc.CurrentTheme.Value = temp[index];
            _state = new PersistableChange<string>(oldValue, newValue);
        }

        return ValueTask.CompletedTask;
    }

    public ValueTask Undo(IRoutable context, CancellationToken cancel = default)
    {
        if (_state != null)
        {
            var theme = _svc.Themes.FirstOrDefault(x => x.Id == _state.OldValue);
            if (theme != null)
            {
                _svc.CurrentTheme.Value = theme;
            }
        }

        return ValueTask.CompletedTask;
    }

    public ValueTask Redo(IRoutable context, CancellationToken cancel = default)
    {
        if (_state != null)
        {
            var theme = _svc.Themes.FirstOrDefault(x => x.Id == _state.NewValue);
            if (theme != null)
            {
                _svc.CurrentTheme.Value = theme;
            }
        }

        return ValueTask.CompletedTask;
    }
}
