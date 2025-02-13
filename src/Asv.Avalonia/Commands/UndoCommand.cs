using System.Composition;
using Avalonia.Input;
using Material.Icons;

namespace Asv.Avalonia;

[ExportCommand]
[Shared]
public class UndoCommandFactory : ICommandFactory
{
    public ICommandInfo Info => UndoCommand.StaticInfo;

    public IAsyncCommand Create(IRoutable context, IPersistable? parameter = null)
    {
        return new UndoCommand(context, parameter);
    }

    public bool CanExecute(IRoutable context, IPersistable? parameter)
    {
        var t = context.FindParentOfType<IPage>();
        if (t != null)
        {
            target = t;
            return true;
        }

        target = context;
        return false;
    }
}

public class UndoCommand : IAsyncCommand
{
    private readonly IRoutable _context;

    public UndoCommand(IRoutable context, IPersistable? parameter)
    {
        _context = context;
    }

    public const string Id = "cmd.global.undo";
    public static ICommandInfo StaticInfo { get; } =
        new CommandInfo
        {
            Id = Id,
            Name = RS.UndoCommand_CommandInfo_Name,
            Description = RS.UndoCommand_CommandInfo_Description,
            Icon = MaterialIconKind.UndoVariant,
            DefaultHotKey = KeyGesture.Parse("Ctrl+Z"),
            Order = 0,
        };

    public ICommandInfo Info => StaticInfo;

    public ValueTask Execute(CancellationToken cancel = default)
    {
        if (_context is IPage page)
        {
            return page.History.UndoAsync(cancel);
        }

        return ValueTask.CompletedTask;
    }
}
