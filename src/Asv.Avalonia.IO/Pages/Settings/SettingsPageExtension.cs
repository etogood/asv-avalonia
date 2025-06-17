using System.Composition;
using Asv.Common;
using Material.Icons;
using Microsoft.Extensions.Logging;
using R3;

namespace Asv.Avalonia.IO;

[ExportExtensionFor<ISettingsPage>]
[method: ImportingConstructor]
public class SettingsPageExtension(ILoggerFactory loggerFactory) : IExtensionFor<ISettingsPage>
{
    public void Extend(ISettingsPage context, CompositeDisposable contextDispose)
    {
        context.Nodes.Add(
            new TreePage(
                SettingsConnectionViewModel.SubPageId,
                RS.SettingsPageExtension_TreePage_Title,
                MaterialIconKind.Connection,
                SettingsConnectionViewModel.SubPageId,
                NavigationId.Empty,
                loggerFactory
            ).DisposeItWith(contextDispose)
        );
    }
}
