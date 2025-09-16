using System.Composition;
using Asv.Common;
using R3;

namespace Asv.Avalonia;

[ExportExtensionFor<IShell>]
[method: ImportingConstructor]
public class MainMenuDefaultMenuExtender(
    [ImportMany(ExportMainMenuAttribute.Contract)] IEnumerable<IMenuItem> items
) : IExtensionFor<IShell>
{
    public void Extend(IShell context, CompositeDisposable contextDispose)
    {
        context.MainMenu.AddRange(items.Select(x => x.DisposeItWith(contextDispose)));
    }
}
