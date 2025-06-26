using System.Composition;
using Microsoft.Extensions.Logging;

namespace Asv.Avalonia;

[ExportMainMenu]
[method: ImportingConstructor]
public class HelpMenu(ILoggerFactory loggerFactory)
    : MenuItem(MenuId, RS.ShellView_Toolbar_Help, loggerFactory)
{
    public const string MenuId = "shell.menu.help";
}
