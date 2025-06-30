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
