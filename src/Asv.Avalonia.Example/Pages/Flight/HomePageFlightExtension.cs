using Asv.Avalonia.Example.Commands;
using Asv.Common;

namespace Asv.Avalonia.Example;

[ExportExtensionFor<IHomePage>]
public class HomePageFlightExtension : AsyncDisposableOnce, IExtensionFor<IHomePage>
{
    public void Extend(IHomePage context)
    {
        context.Tools.Add(OpenFlightModeCommand.StaticInfo.CreateAction());
    }
}
