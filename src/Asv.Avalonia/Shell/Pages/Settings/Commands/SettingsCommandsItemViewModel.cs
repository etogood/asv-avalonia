namespace Asv.Avalonia.Commands;

public class SettingsCommandsItemViewModel : RoutableViewModel
{
    private readonly ICommandService _svc;
    private bool _isSelected;
    private HotKeyInfo? _currentHotKey;

    public SettingsCommandsItemViewModel(ICommandInfo cmd, ICommandService svc)
        : base(cmd.Id)
    {
        _svc = svc;
        CurrentHotKey = _svc.GetHotKey(cmd.Id);
        Info = cmd;
    }

    public ICommandInfo Info { get; }

    public HotKeyInfo? CurrentHotKey
    {
        get => _currentHotKey;
        set => SetField(ref _currentHotKey, value);
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
        get => _isSelected;
        set => SetField(ref _isSelected, value);
    }
}
