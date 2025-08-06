using Microsoft.Extensions.Logging;

namespace Asv.Avalonia;

public class LoggerOptions
{
    public const string ConfigurationSection = "Logs";

    public required bool? ViewerEnabled { get; init; } = false;
    public required bool? LogToConsole { get; init; } = false;
    public required LogToFileOptions? LogToFileOptions { get; init; }
    public required LogLevel? Level { get; init; }
}
