using System.Composition;
using System.Reactive.Disposables;
using Avalonia.Input;

namespace Asv.Avalonia.LogViewer;

[ExportCommand]
[method: ImportingConstructor]
public class OpenLogViewerCommand(INavigationService nav)
    : OpenPageCommandBase(LogViewerViewModel.PageId, nav)
{
    #region Static

    public const string Id = $"{BaseId}.open.{LogViewerViewModel.PageId}";

    public static readonly ICommandInfo StaticInfo = new CommandInfo // TODO: Localize
    {
        Id = Id,
        Name = "Log Viewer",
        Description = "Open log viewer",
        Icon = LogViewerViewModel.PageIcon,
        Source = SystemModule.Instance,
        DefaultHotKey = "Ctrl+Shift+L",
    };

    #endregion
    public override ICommandInfo Info => StaticInfo;
}
