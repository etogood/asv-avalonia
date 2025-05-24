namespace Asv.Avalonia.LogViewer;

public class LogViewerModule : IExportInfo
{
    public const string Name = "Asv.Avalonia.LogViewer";
    public static IExportInfo Instance { get; } = new LogViewerModule();

    private LogViewerModule() { }

    public string ModuleName => Name;
}
