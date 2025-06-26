using System.Composition;
using Microsoft.Extensions.Logging;

namespace Asv.Avalonia;

[ExportMainMenu]
[method: ImportingConstructor]
public class ToolsMenu(ILoggerFactory loggerFactory)
    : MenuItem(MenuId, RS.ToolsMenu_Name, loggerFactory)
{
    public const string MenuId = "shell.menu.tools";
}
