using System.Composition;
using Material.Icons;
using Microsoft.Extensions.Logging;

namespace Asv.Avalonia;

[ExportMainMenu]
public class ViewMenu : MenuItem
{
    [method: ImportingConstructor]
    public ViewMenu(ILoggerFactory loggerFactory)
        : base(MenuId, RS.ShellView_Toolbar_View, loggerFactory)
    {
        Order = 80;
        Icon = MaterialIconKind.ViewGrid;
    }

    public const string MenuId = "shell.menu.view";
}
