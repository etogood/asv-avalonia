using System.Composition;
using Material.Icons;
using Microsoft.Extensions.Logging;

namespace Asv.Avalonia;

[ExportMainMenu]
public class HelpMenu : MenuItem
{
    [method: ImportingConstructor]
    public HelpMenu(ILoggerFactory loggerFactory)
        : base(MenuId, RS.ShellView_Toolbar_Help, loggerFactory)
    {
        Order = 100;
        Icon = MaterialIconKind.Help;
    }

    public const string MenuId = "shell.menu.help";
}
