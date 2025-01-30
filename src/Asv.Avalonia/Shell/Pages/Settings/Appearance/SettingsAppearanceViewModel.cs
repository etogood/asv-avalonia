using System.Composition;
using R3;

namespace Asv.Avalonia;

[ExportExtensionFor<ISettingsPage>]
[method: ImportingConstructor]
public class SettingsExtension() : IExtensionFor<ISettingsPage>
{
    private TreePageNode? _node1;

    public void Extend(ISettingsPage context)
    {
        _node1 = new TreePageNode(SettingsAppearanceViewModel.PageId, SettingsAppearanceViewModel.PageId );
        context.Nodes.Add(_node1);
    }

    public void Dispose()
    {
        _node1?.Dispose();
    }
}

[ExportSettings(PageId)]
public class SettingsAppearanceViewModel : RoutableViewModel, ISettingsSubPage
{
    public const string PageId = "appearance";

    #region DesignTime

    public SettingsAppearanceViewModel()
        : this(DesignTime.ThemeService)
    {
        DesignTime.ThrowIfNotDesignMode();
    }

    #endregion

    [ImportingConstructor]
    public SettingsAppearanceViewModel(IThemeService themeService)
        : base(PageId)
    {
        Theme = new ThemeProperty(themeService) { NavigationParent = this };
    }

    public ThemeProperty Theme { get; }
    public ValueTask Init(ISettingsPage context)
    {
        return ValueTask.CompletedTask;
    }
}