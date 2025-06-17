using System.Composition;
using Microsoft.Extensions.Logging;

namespace Asv.Avalonia;

[ExportMainMenu]
public class EditMenu : MenuItem
{
    public const string MenuId = "shell.menu.edit";

    [ImportingConstructor]
    public EditMenu(ILoggerFactory loggerFactory)
        : base(MenuId, RS.ShellView_Toolbar_Edit, loggerFactory) { }
}

[ExportMainMenu]
public class EditUndoMenu : MenuItem
{
    public const string MenuId = $"{EditMenu.MenuId}.undo";

    [ImportingConstructor]
    public EditUndoMenu(IShellHost host, ILoggerFactory loggerFactory)
        : base(MenuId, RS.UndoCommand_CommandInfo_Name, loggerFactory, EditMenu.MenuId)
    {
        Command = new BindableAsyncCommand(UndoCommand.Id, this);
        Order = 0;
    }
}

[ExportMainMenu]
public class EditRedoMenu : MenuItem
{
    public const string MenuId = $"{EditMenu.MenuId}.redo";

    [ImportingConstructor]
    public EditRedoMenu(ILoggerFactory loggerFactory)
        : base(MenuId, RS.RedoCommand_CommandInfo_Name, loggerFactory, EditMenu.MenuId)
    {
        Command = new BindableAsyncCommand(RedoCommand.Id, this);
        Order = 1;
    }
}
