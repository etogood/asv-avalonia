using System.Composition;
using Microsoft.Extensions.Logging;

namespace Asv.Avalonia;

[ExportMainMenuAttribute]
public class OpenMenu : MenuItem
{
    public const string MenuId = "shell.menu.open";

    [ImportingConstructor]
    public OpenMenu(ILoggerFactory loggerFactory, ICommandService cmd)
        : base(MenuId, RS.ShellView_Toolbar_Open, loggerFactory)
    {
        Order = -100;
        Icon = OpenFileCommand.StaticInfo.Icon;
        HotKey = cmd.GetHotKey(OpenFileCommand.Id)?.Gesture;
        Command = new BindableAsyncCommand(OpenFileCommand.Id, this);
    }
}
