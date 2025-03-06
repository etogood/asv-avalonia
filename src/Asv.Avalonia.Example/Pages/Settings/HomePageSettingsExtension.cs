using Asv.Common;

namespace Asv.Avalonia.Example;

[ExportExtensionFor<IHomePage>]
public class HomePageSettingsExtension : AsyncDisposableOnce, IExtensionFor<IHomePage>
{
    public void Extend(IHomePage context)
    {
        context.Tools.Add(OpenSettingsCommand.StaticInfo.CreateAction());
    }
}
