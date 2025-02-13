using Avalonia.Input;
using Material.Icons;

namespace Asv.Avalonia;

[ExportCommand]
public class OpenSettingsCommandFactory : ICommandFactory
{
    public ICommandInfo Info => OpenSettingsCommand.StaticInfo;

    public IAsyncCommand Create(IRoutable context, IPersistable? parameter = null)
    {
        return new OpenSettingsCommand(context, parameter);
    }

    public bool CanExecute(IRoutable context, IPersistable? parameter)
    {
        var t = context.FindParentOfType<IShell>();
        if (t == null)
        {
            target = context;
            return false;
        }

        target = t;
        return true;
    }
}

public class OpenSettingsCommand : IAsyncCommand
{
    private readonly IRoutable _context;

    public OpenSettingsCommand(IRoutable context, IPersistable? parameter)
    {
        _context = context;
    }

    public const string Id = "cmd.open.settings";

    public static readonly ICommandInfo StaticInfo = new CommandInfo
    {
        Id = Id,
        Name = "Settings",
        Description = "Open settings",
        Icon = MaterialIconKind.Cog,
        DefaultHotKey = KeyGesture.Parse("Ctrl+F1"),
        Order = 0,
    };

    public ICommandInfo Info => StaticInfo;

    public async ValueTask Execute(CancellationToken cancel = default)
    {
        var shell = _context.FindParentOfType<IShell>();
        if (shell != null)
        {
            await shell.Navigate(SettingsPageViewModel.PageId);
        }
    }
}
