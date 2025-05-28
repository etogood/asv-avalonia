using Microsoft.Extensions.Options;

namespace Asv.Avalonia.Plugins;

/// <summary>
/// Immutable, validated configuration for the <c>PluginManager</c>.
/// Values are typically populated from the <c>"Plugins"</c> section of <c>appsettings.*.json</c>,
/// then optionally overridden by <see cref="PluginManagerBuilder"/> at runtime.
/// </summary>
public sealed record PluginManagerOptions
{
    /// <summary>Name of the configuration section.</summary>
    public const string Section = "Plugins";

    /// <summary>Gets or sets unique salt used to derive plugin‑specific paths (e.g., for hashing).</summary>
    public required string Salt { get; set; }

    /// <summary>Gets or sets absolute path where extracted plugin folders are stored.</summary>
    public required string PluginDirectory { get; set; }

    /// <summary>Gets or sets package ID of the API assembly that every plugin must reference.</summary>
    public required string ApiPackageId { get; set; }

    /// <summary>Gets or sets package ID prefix that marks NuGet packages as plugins.</summary>
    public required string NugetPluginPrefix { get; set; }

    /// <summary>Gets or sets semantic version of the API assembly required by the host.</summary>
    public required string ApiVersion { get; set; }

    /// <summary>Gets or sets absolute path where downloaded <c>.nupkg</c> files are kept.</summary>
    public required string NugetDirectory { get; set; }

    /// <summary>Gets or sets absolute path for the HTTP cache used by NuGet client.</summary>
    public required string NugetCacheDirectory { get; set; }

    /// <summary>Gets or sets default set of NuGet servers used for plugin discovery.</summary>
    public IReadOnlyList<PluginServer> DefaultServers { get; set; } = [];
}

/// <summary>
/// Validates <see cref="PluginManagerOptions"/> at DI start‑up (see <c>ValidateOnStart</c> in <see cref="PluginManagerMixin"/>).
/// </summary>
internal sealed class PluginManagerOptionsValidator : IValidateOptions<PluginManagerOptions>
{
    public ValidateOptionsResult Validate(string? name, PluginManagerOptions options)
    {
        var errors = new List<string>();

        Check(options.Salt, nameof(options.Salt), errors);
        Check(options.PluginDirectory, nameof(options.PluginDirectory), errors);
        Check(options.ApiPackageId, nameof(options.ApiPackageId), errors);
        Check(options.NugetPluginPrefix, nameof(options.NugetPluginPrefix), errors);
        Check(options.ApiVersion, nameof(options.ApiVersion), errors);
        Check(options.NugetDirectory, nameof(options.NugetDirectory), errors);
        Check(options.NugetCacheDirectory, nameof(options.NugetCacheDirectory), errors);

        if (options.DefaultServers.Count == 0)
        {
            errors.Add("At least one default plugin server must be configured.");
        }

        return errors.Count == 0
            ? ValidateOptionsResult.Success
            : ValidateOptionsResult.Fail(errors);

        static void Check(string value, string field, ICollection<string> target)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                target.Add($"Configuration value '{field}' is required but was empty.");
            }
        }
    }
}
