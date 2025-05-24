using System.Composition;

namespace Asv.Avalonia.LogViewer;

[ExportCommand]
[method: ImportingConstructor]
public class OpenLogViewerPageCommand(INavigationService nav)
    : OpenPageCommandBase(LogViewerViewModel.PageId, nav)
{
    #region Static

    public const string Id = $"{BaseId}.open.logview";

    public static readonly ICommandInfo StaticInfo = new CommandInfo // TODO: Localize
    {
        Id = Id,
        Name = "Log Viewer",
        Description = "Open log viewer",
        Icon = LogViewerViewModel.PageIcon,
        DefaultHotKey = null,
        Source = LogViewerModule.Instance,
    };

    #endregion
    public override ICommandInfo Info => StaticInfo;
}
