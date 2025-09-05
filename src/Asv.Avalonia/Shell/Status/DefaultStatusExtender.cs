using System.Composition;
using Asv.Common;

namespace Asv.Avalonia;

[ExportExtensionFor<IShell>]
[method: ImportingConstructor]
public class DefaultStatusExtender(
    [ImportMany(ExportStatusItemAttribute.Contract)] IEnumerable<IStatusItem> items
) : IExtensionFor<IShell>
{
    public void Extend(IShell context, R3.CompositeDisposable contextDispose)
    {
        context.StatusItems.AddRange(items.Select(x => x.DisposeItWith(contextDispose)));
    }
}
