using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Asv.Avalonia.Plugins;

/// <summary>
/// Extension methods for <see cref="IHostApplicationBuilder"/> that plug the ASV plugin‑system into the DI‑pipeline.
/// </summary>
/// <remarks>
/// Typical usage from <c>Program.cs</c> (minimal hosting):
/// <code>
/// var builder = Host.CreateApplicationBuilder(args)
///                 .UsePluginManager(b => b
///                     .WithApiPackage(typeof(IMyApiMarker).Assembly)
///                     .WithPluginPrefix("Asv.MyProduct.Plugin."));
/// </code>
/// The call is idempotent – multiple invocations will not register duplicates.
/// </remarks>
public static class PluginManagerMixin
{
    /// <summary>
    /// Registers <see cref="PluginManager"/> and configures its <see cref="PluginManagerOptions"/>.
    /// </summary>
    /// <param name="builder">The host‑builder being configured.</param>
    /// <param name="configureBuilder">Optional delegate that fills <see cref="PluginManagerBuilder"/> with custom values.</param>
    /// <param name="postConfigureOptions">Optional delegate that post‑processes <see cref="PluginManagerOptions"/> after the builder has populated them (e.g. for environment overrides).</param>
    /// <returns>The same <paramref name="builder"/> instance so that calls can be chained.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="builder"/> is <c>null</c>.</exception>
    public static IHostApplicationBuilder UsePluginManager(
        this IHostApplicationBuilder builder,
        Action<PluginManagerBuilder>? configureBuilder = null,
        Action<PluginManagerOptions>? postConfigureOptions = null
    )
    {
        ArgumentNullException.ThrowIfNull(builder);
        var opts = builder
            .Services.AddOptions<PluginManagerOptions>()
            .Bind(builder.Configuration.GetSection(PluginManagerOptions.Section));

        if (postConfigureOptions is not null)
        {
            opts.Configure(postConfigureOptions);
        }

        var pmBuilder = new PluginManagerBuilder();
        configureBuilder?.Invoke(pmBuilder);
        pmBuilder.Build(opts);

        builder.Services.TryAddEnumerable(
            ServiceDescriptor.Singleton<
                IValidateOptions<PluginManagerOptions>,
                PluginManagerOptionsValidator
            >()
        );
        opts.ValidateOnStart();

        builder.Services.TryAddSingleton<IPluginManager, PluginManager>();

        return builder;
    }
}
