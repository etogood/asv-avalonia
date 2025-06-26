using System.Composition;
using Microsoft.Extensions.Logging;

namespace Asv.Avalonia;

[ExportMainMenu]
[method: ImportingConstructor]
public class ViewMenu(ILoggerFactory loggerFactory)
    : MenuItem(MenuId, RS.ShellView_Toolbar_View, loggerFactory)
{
    public const string MenuId = "shell.menu.view";
}
