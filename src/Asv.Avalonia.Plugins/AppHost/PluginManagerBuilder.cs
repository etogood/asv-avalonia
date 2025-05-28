using System.Reflection;
using Asv.Common;
using Microsoft.Extensions.Options;

namespace Asv.Avalonia.Plugins;

public sealed class PluginManagerBuilder
{
    private string _apiPackageName = string.Empty;
    private SemVersion _apiVersion = "0.0.0";
    private string _nugetPluginPrefix = string.Empty;
    private readonly HashSet<PluginServer> _servers = new(
        EqualityComparer<PluginServer>.Create((a, b) => a?.SourceUri == b?.SourceUri)
    );
    private string _relativePluginFolder = "plugins";
    private string _relativeNugetFolder = "nuget";
    private string _relativeNugetCacheFolder = "nuget_cache";
    private string _salt = "Asv.Avalonia.Plugins";

    internal PluginManagerBuilder()
    {
        _servers.Add(new PluginServer("NuGet", "https://api.nuget.org/v3/index.json"));
    }

    /// <summary>
    /// Sets the API package name for the Plugin Manager that an application uses as a shared package, linking plugins.
    /// </summary>
    /// <param name="apiPackageName">The API package name to be set.</param>
    /// <param name="apiVersion">The API version to be set.</param>
    public PluginManagerBuilder WithApiPackage(string apiPackageName, SemVersion apiVersion)
    {
        ArgumentNullException.ThrowIfNull(apiPackageName);
        ArgumentNullException.ThrowIfNull(apiVersion);
        _apiPackageName = apiPackageName;
        _apiVersion = apiVersion;
        return this;
    }

    public PluginManagerBuilder WithApiPackage(Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);
        _apiPackageName = assembly.GetName().Name ?? throw new InvalidOperationException();
        var attributes = assembly.GetCustomAttributes(
            typeof(AssemblyInformationalVersionAttribute),
            false
        );

        ArgumentNullException.ThrowIfNull(attributes);
        if (attributes.Length == 0)
        {
            throw new ArgumentNullException(nameof(attributes));
        }

        var nameAttribute = (AssemblyInformationalVersionAttribute)attributes[0];
        ArgumentException.ThrowIfNullOrEmpty(nameAttribute.InformationalVersion);

        _apiVersion = SemVersion.Parse(nameAttribute.InformationalVersion);

        return this;
    }

    /// <summary>
    /// Sets the plugin package prefix that will be used as a NuGet search filter, e.g. "Asv.Avalonia.Plugin.".
    /// </summary>
    /// <param name="pluginPrefix">The NuGet package prefix to be set.</param>
    public PluginManagerBuilder WithPluginPrefix(string pluginPrefix)
    {
        ArgumentNullException.ThrowIfNull(pluginPrefix);
        _nugetPluginPrefix = pluginPrefix;

        return this;
    }

    /// <summary>
    /// Adds a NuGet plugins server to the Plugin Manager's list of servers if it doesn't already exist.
    /// </summary>
    /// <param name="server">The NuGet plugins server to be added.</param>
    public PluginManagerBuilder WithServer(PluginServer server)
    {
        ArgumentNullException.ThrowIfNull(server);
        _servers.Add(server);

        return this;
    }

    public PluginManagerBuilder WithRelativePluginFolder(string pluginFolder)
    {
        ArgumentNullException.ThrowIfNull(pluginFolder);
        _relativePluginFolder = pluginFolder;
        return this;
    }

    public PluginManagerBuilder WithRelativeNugetFolder(string nugetFolder)
    {
        ArgumentNullException.ThrowIfNull(nugetFolder);
        _relativeNugetFolder = nugetFolder;
        return this;
    }

    public PluginManagerBuilder WithRelativeNugetCacheFolder(string nugetCacheFolder)
    {
        ArgumentNullException.ThrowIfNull(nugetCacheFolder);
        _relativeNugetCacheFolder = nugetCacheFolder;
        return this;
    }

    public PluginManagerBuilder WithSalt(string salt)
    {
        ArgumentNullException.ThrowIfNull(salt);
        _salt = salt;
        return this;
    }

    internal OptionsBuilder<PluginManagerOptions> Build(
        OptionsBuilder<PluginManagerOptions> options
    )
    {
        Validate();

        return options.Configure(
            (PluginManagerOptions cfg, IAppPath path) =>
            {
                cfg.PluginDirectory = Path.Join(path.UserDataFolder, _relativePluginFolder);
                cfg.NugetDirectory = Path.Join(path.UserDataFolder, _relativeNugetFolder);
                cfg.NugetCacheDirectory = Path.Join(path.UserDataFolder, _relativeNugetCacheFolder);
                cfg.ApiVersion = _apiVersion.ToString();
                cfg.ApiPackageId = _apiPackageName;
                cfg.NugetPluginPrefix = _nugetPluginPrefix;
                cfg.Salt = _salt;
                cfg.DefaultServers = _servers.ToList();
            }
        );
    }

    private void Validate()
    {
        if (string.IsNullOrWhiteSpace(_apiPackageName))
        {
            throw new InvalidOperationException(
                "API package name is not configured — call WithApiPackage first."
            );
        }

        if (_apiVersion == "0.0.0")
        {
            throw new InvalidOperationException(
                "API version is not configured — call WithApiPackage first."
            );
        }
    }
}
