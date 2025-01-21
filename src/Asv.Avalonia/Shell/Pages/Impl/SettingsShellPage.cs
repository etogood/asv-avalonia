namespace Asv.Avalonia;

[ExportShellPage(PageId)]
public class SettingsShellPage : ShellPage
{
    public const string PageId = "settings";
    public SettingsShellPage()
        : base(PageId)
    {
    }

    public override IEnumerable<IRoutableViewModel> Children { get; } = [];
    protected override ValueTask InternalCatchEvent(AsyncRoutedEvent asyncRoutedEvent)
    {
        return ValueTask.CompletedTask;
    }
}