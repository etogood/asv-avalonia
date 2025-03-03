using System.Composition;

namespace Asv.Avalonia;

[ExportSettings(PageId)]
public class SettingsAppearanceViewModel : RoutableViewModel, ISettingsSubPage
{
    public const string PageId = "appearance";

    #region DesignTime

    public SettingsAppearanceViewModel()
        : this(DesignTime.ThemeService, DesignTime.LocalizationService, null!)
    {
        DesignTime.ThrowIfNotDesignMode();
    }

    #endregion

    [ImportingConstructor]
    public SettingsAppearanceViewModel(
        IThemeService themeService,
        ILocalizationService localizationService,
        IDialogService dialogService
    )
        : base(PageId)
    {
        Theme = new ThemeProperty(themeService) { Parent = this };
        Language = new LanguageProperty(localizationService, dialogService) { Parent = this };
    }

    public ThemeProperty Theme { get; }
    public LanguageProperty Language { get; }

    public ValueTask Init(ISettingsPage context)
    {
        return ValueTask.CompletedTask;
    }

    public override IEnumerable<IRoutable> GetRoutableChildren()
    {
        yield return Theme;
        yield return Language;
    }

    public IExportInfo Source => SystemModule.Instance;
}
