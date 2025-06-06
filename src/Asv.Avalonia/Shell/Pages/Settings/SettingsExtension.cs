using System.Composition;
using Asv.Common;
using Material.Icons;
using R3;

namespace Asv.Avalonia;

[ExportExtensionFor<ISettingsPage>]
[method: ImportingConstructor]
public class SettingsExtension() : IExtensionFor<ISettingsPage>
{
    public void Extend(ISettingsPage context, CompositeDisposable contextDispose)
    {
        context.Nodes.Add(
            new TreePage(
                SettingsAppearanceViewModel.PageId,
                RS.SettingsAppearanceViewModel_Name,
                MaterialIconKind.ThemeLightDark,
                SettingsAppearanceViewModel.PageId,
                Routable.NavigationId.Empty
            ).DisposeItWith(contextDispose)
        );

        context.Nodes.Add(
            new TreePage(
                SettingsUnitsViewModel.PageId,
                RS.SettingsUnitsViewModel_Name,
                MaterialIconKind.TemperatureCelsius,
                SettingsUnitsViewModel.PageId,
                Routable.NavigationId.Empty
            ).DisposeItWith(contextDispose)
        );

        context.Nodes.Add(
            new TreePage(
                SettingsCommandsViewModel.SubPageId,
                RS.SettingsCommandListViewModel_Name,
                MaterialIconKind.KeyboardSettings,
                SettingsCommandsViewModel.SubPageId,
                Routable.NavigationId.Empty
            ).DisposeItWith(contextDispose)
        );
    }
}
