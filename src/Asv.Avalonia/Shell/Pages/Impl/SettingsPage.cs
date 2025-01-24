using System.Composition;
using R3;

namespace Asv.Avalonia;

[ExportShellPage(PageId)]
public class SettingsPage : ShellPage
{
    private readonly BindableReactiveProperty<string> _title = new(PageId);
    public const string PageId = "settings";

    public SettingsPage()
        : base(PageId, DesignTime.CommandService)
    {
        DesignTime.ThrowIfNotDesignMode();
    }

    [ImportingConstructor]
    public SettingsPage(IThemeService themeService, ICommandService svc)
        : base(PageId, svc)
    {
        Theme = new ThemePropertyViewModel(themeService)
        {
            Parent = this,
        };
    }

    public ThemePropertyViewModel Theme { get; }

    public override IEnumerable<IRoutableViewModel> Children
    {
        get
        {
            yield return Theme;
        }
    }

    public override IReadOnlyBindableReactiveProperty<string> Title => _title;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _title.Dispose();
            Theme.Dispose();
        }

        base.Dispose(disposing);
    }
}