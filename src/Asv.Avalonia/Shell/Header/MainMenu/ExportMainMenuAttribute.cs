using System.Composition;
using Asv.Common;
using R3;

namespace Asv.Avalonia;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class ExportMainMenuAttribute() : ExportAttribute(Contract, typeof(IMenuItem))
{
    public const string Contract = "shell.menu.main";
}

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
