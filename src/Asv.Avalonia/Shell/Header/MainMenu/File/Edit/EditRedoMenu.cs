using System.Composition;
using Microsoft.Extensions.Logging;

namespace Asv.Avalonia;

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
