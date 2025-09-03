using System.Composition;
using Asv.Avalonia.FileAssociation;
using Microsoft.Extensions.Logging;

namespace Asv.Avalonia;

[ExportMainMenuAttribute]
public class OpenMenu : MenuItem
{
    public const string MenuId = "shell.menu.open";

    [ImportingConstructor]
    public OpenMenu(ILoggerFactory loggerFactory, IFileAssociationService svc)
        : base(MenuId, RS.ShellView_Toolbar_Open, loggerFactory)
    {
        Order = -1;
        IsVisible = svc.HasAnyHandlersForOpenFile;
        Command = new BindableAsyncCommand(OpenFileCommand.Id, this);
    }
}
