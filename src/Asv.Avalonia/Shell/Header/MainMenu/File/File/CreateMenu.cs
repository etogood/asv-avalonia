using System.Composition;
using System.Reactive.Disposables;
using Material.Icons;
using Microsoft.Extensions.Logging;

namespace Asv.Avalonia;

[ExportMainMenu]
public class CreateMenu : MenuItem
{
    public const string MenuId = "shell.menu.create";

    [ImportingConstructor]
    public CreateMenu(ILoggerFactory loggerFactory)
        : base(MenuId, "Create", loggerFactory)
    {
        Order = -90;
        Icon = MaterialIconKind.FilePlus;
        Header = "Create";
    }
}
