using Asv.Common;

namespace Asv.Avalonia;

public interface IAppInfo
{
    /// <summary>
    /// Application name
    /// </summary>
    string Title { get; }

    /// <summary>
    /// Application name
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Application version
    /// </summary>
    string Version { get; }

    /// <summary>
    /// Authors
    /// </summary>
    string CompanyName { get; }

    /// <summary>
    /// Avalonia UI package version
    /// </summary>
    string AvaloniaVersion { get; }
}
