using System.Composition;
using Avalonia.Input;
using Material.Icons;

namespace Asv.Avalonia;

[ExportCommand]
[Shared]
public class ClosePageCommandFactory : ICommandFactory
{
    public ICommandInfo Info => ClosePageCommand.StaticInfo;

    public IAsyncCommand Create(IRoutable context, IPersistable? parameter = null)
    {
        return new ClosePageCommand(context);
    }

    public bool CanExecute(IRoutable context, IPersistable? parameter)
    {
        var t = context.FindParentOfType<IShell>()?.SelectedPage.Value;
        if (t == null)
        {
            target = context;
            return false;
        }

        target = t;
        return true;
    }

    public bool CanExecute(IRoutable? context, out IRoutable? target)
    {
        target = context.FindParentOfType<IShell>()?.SelectedPage.Value;
        return target != null;
    }
}

public class ClosePageCommand : IAsyncCommand
{
    private readonly IRoutable _context;

    public ClosePageCommand(IRoutable context)
    {
        _context = context;
    }

    public const string Id = "cmd.page.close";

    public static readonly ICommandInfo StaticInfo = new CommandInfo
    {
        Id = Id,
        Name = "Close page",
        Description = RS.OpenDebugCommand_CommandInfo_Description,
        Icon = MaterialIconKind.CloseBold,
        DefaultHotKey = KeyGesture.Parse("Alt+F4"),
        Order = 0,
    };

    public ICommandInfo Info => StaticInfo;

    public ValueTask Execute(CancellationToken cancel = default)
    {
        if (_context is IPage page)
        {
            return page.TryCloseAsync();
        }

        return ValueTask.CompletedTask;
    }
}
