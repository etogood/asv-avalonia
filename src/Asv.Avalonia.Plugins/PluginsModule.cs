namespace Asv.Avalonia.Plugins;

public class PluginsModule : IExportInfo
{
    public const string Name = "Asv.Avalonia.Plugins";
    public static IExportInfo Instance { get; } = new PluginsModule();

    private PluginsModule() { }

    public string ModuleName => Name;
}
