using System.Composition;
using Microsoft.Extensions.Logging;

namespace Asv.Avalonia;

[ExportMainMenuAttribute]
[method: ImportingConstructor]
public class FileMenu(ILoggerFactory loggerFactory)
    : MenuItem(MenuId, RS.ShellView_Toolbar_File, loggerFactory)
{
    public const string MenuId = "shell.menu.file";
}
