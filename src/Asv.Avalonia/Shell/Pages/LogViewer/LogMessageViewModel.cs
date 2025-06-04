using Material.Icons;
using Microsoft.Extensions.Logging;

namespace Asv.Avalonia.LogViewer;

public class LogMessageViewModel : RoutableViewModel
{
    public LogMessageViewModel(LogMessage @base, IRoutable parent)
        : base(
            HashCode
                .Combine(@base.Message, @base.Category, @base.Message, @base.Description)
                .ToString()
        )
    {
        Base = @base;
        Parent = parent;
    }

    public LogMessage Base { get; }

    public MaterialIconKind Icon =>
        Base.LogLevel switch
        {
            LogLevel.Critical => MaterialIconKind.AlertCircle,
            LogLevel.Error => MaterialIconKind.Alert,
            LogLevel.Information => MaterialIconKind.Information,
            LogLevel.Warning => MaterialIconKind.AlertOutline,
            LogLevel.Trace => MaterialIconKind.Bug,
            LogLevel.Debug => MaterialIconKind.CodeBraces,
            _ => MaterialIconKind.HelpCircle,
        };

    public bool IsTrace => Base.LogLevel is LogLevel.Trace or LogLevel.None;
    public bool IsDebug => Base.LogLevel == LogLevel.Debug;
    public bool IsInformation => Base.LogLevel == LogLevel.Information;
    public bool IsWarning => Base.LogLevel == LogLevel.Warning;
    public bool IsError => Base.LogLevel is LogLevel.Error or LogLevel.Critical;
    public string DateTime => Base.Timestamp.ToString("yy-MM-dd HH:mm:ss.fff");

    public override IEnumerable<IRoutable> GetRoutableChildren()
    {
        yield break;
    }

    protected override void Dispose(bool disposing)
    {
        // No resources to dispose
    }
}
