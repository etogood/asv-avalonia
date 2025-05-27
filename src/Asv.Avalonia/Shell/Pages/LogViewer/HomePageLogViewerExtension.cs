using Asv.Common;

namespace Asv.Avalonia.LogViewer;

[ExportExtensionFor<IHomePage>]
public sealed class HomePageLogViewerExtension : IExtensionFor<IHomePage>
{
    public void Extend(IHomePage context, R3.CompositeDisposable contextDispose)
    {
        context.Tools.Add(
            OpenLogViewerCommand.StaticInfo.CreateAction().DisposeItWith(contextDispose)
        );
    }
}
