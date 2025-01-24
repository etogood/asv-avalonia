using System.Composition;
using R3;

namespace Asv.Avalonia;

[ExportExtensionFor<ISettingsPage>]
[method: ImportingConstructor]
public class SettingsExtension(IThemeService theme) : IExtensionFor<ISettingsPage>
{
    private TreePageNode? _node1;

    public ValueTask Extend(ISettingsPage viewModel)
    {
        _node1 = new TreePageNode(SettingsAppearanceViewModel.PageId, () => new SettingsAppearanceViewModel(theme, viewModel) );
        viewModel.Nodes.Add(_node1);
        return ValueTask.CompletedTask;
    }

    public void Dispose()
    {
        _node1?.Dispose();
    }
}

public class SettingsAppearanceViewModel : RoutableViewModel
{
    public const string PageId = "settings.appearance";

    #region DesignTime

    public SettingsAppearanceViewModel()
        : this(DesignTime.ThemeService, new SettingsPageViewModel())
    {
        DesignTime.ThrowIfNotDesignMode();
    }

    #endregion

    public SettingsAppearanceViewModel(IThemeService themeService, ISettingsPage context)
        : base(PageId)
    {
        Theme = new ThemeProperty(themeService) { Parent = this };
    }

    public ThemeProperty Theme { get; }
}