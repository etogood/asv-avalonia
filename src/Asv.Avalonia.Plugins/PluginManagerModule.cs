namespace Asv.Avalonia.Plugins;

public sealed class PluginManagerModule : IExportInfo
{
    public const string Name = "Asv.Avalonia.Plugins";
    public static IExportInfo Instance { get; } = new PluginManagerModule();

    private PluginManagerModule() { }

    public string ModuleName => Name;
}
