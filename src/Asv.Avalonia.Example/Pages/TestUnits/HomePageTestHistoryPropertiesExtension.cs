using Asv.Common;
using R3;

namespace Asv.Avalonia.Example;

[ExportExtensionFor<IHomePage>]
public class HomePageTestHistoryPropertiesExtension : AsyncDisposableOnce, IExtensionFor<IHomePage>
{
    public void Extend(IHomePage context, CompositeDisposable contextDispose)
    {
        context.Tools.Add(
            OpenTestHistoryPropertiesPageCommand
                .StaticInfo.CreateAction()
                .DisposeItWith(contextDispose)
        );
    }
}
