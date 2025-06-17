using Microsoft.Extensions.Logging;

namespace Asv.Avalonia;

public class SettingsCommandsItemViewModel : RoutableViewModel
{
    public SettingsCommandsItemViewModel(
        ICommandInfo cmd,
        ICommandService svc,
        ILoggerFactory loggerFactory
    )
        : base(cmd.Id, loggerFactory)
    {
        CurrentHotKey = svc.GetHotKey(cmd.Id);
        Info = cmd;
    }

    public ICommandInfo Info { get; }

    public HotKeyInfo? CurrentHotKey
    {
        get;
        set => SetField(ref field, value);
    }

    public bool Filter(string text)
    {
        return Info.DefaultHotKey?.ToString().Contains(text, StringComparison.OrdinalIgnoreCase)
                == true
            || CurrentHotKey?.ToString().Contains(text, StringComparison.OrdinalIgnoreCase) == true
            || Info.Name.Contains(text, StringComparison.OrdinalIgnoreCase)
            || Info.Source.ModuleName.Contains(text, StringComparison.OrdinalIgnoreCase)
            || Info.Description.Contains(text, StringComparison.OrdinalIgnoreCase);
    }

    public override IEnumerable<IRoutable> GetRoutableChildren()
    {
        yield break;
    }

    public bool IsSelected
    {
        get;
        set => SetField(ref field, value);
    }
}
