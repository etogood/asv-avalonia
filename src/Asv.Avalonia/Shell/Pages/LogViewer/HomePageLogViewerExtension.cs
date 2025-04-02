using Asv.Common;
using R3;

namespace Asv.Avalonia;

[ExportExtensionFor<IHomePage>]
public class HomePageLogViewerExtension : IExtensionFor<IHomePage>
{
    public void Extend(IHomePage context, CompositeDisposable contextDispose)
    {
        context.Tools.Add(
            OpenLogViewerPageCommand.StaticInfo.CreateAction().DisposeItWith(contextDispose)
        );
    }
}