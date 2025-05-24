using Asv.Common;
using R3;

namespace Asv.Avalonia.LogViewer;

[ExportExtensionFor<IHomePage>]
public sealed class HomePageLogViewerExtension : IExtensionFor<IHomePage>
{
    public void Extend(IHomePage context, CompositeDisposable contextDispose)
    {
        context.Tools.Add(
            OpenLogViewerPageCommand.StaticInfo.CreateAction().DisposeItWith(contextDispose)
        );
    }
}
