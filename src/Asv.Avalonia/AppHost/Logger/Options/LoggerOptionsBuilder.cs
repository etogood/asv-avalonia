using Microsoft.Extensions.Logging;

namespace Asv.Avalonia;

public class LoggerOptionsBuilder
{
    private LogLevel? _level;
    private bool _logToConsole;
    private LogToFileOptions? _logToFileOptions;
    private bool _viewerEnabled;

    internal LoggerOptionsBuilder() { }

    internal LoggerOptionsBuilder(LoggerOptions defaultOptions)
    {
        _viewerEnabled = defaultOptions.ViewerEnabled ?? _viewerEnabled;
        _logToConsole = defaultOptions.LogToConsole ?? _logToConsole;
        _logToFileOptions = defaultOptions.LogToFileOptions;
        _level = defaultOptions.Level;
    }

    public LoggerOptionsBuilder WithLogToConsole()
    {
        _logToConsole = true;
        return this;
    }

    public LoggerOptionsBuilder WithLogToFile(string? folderName = null, int? rollingSizeKb = null)
    {
        _logToFileOptions = new LogToFileOptions(folderName, rollingSizeKb);
        return this;
    }

    public LoggerOptionsBuilder WithLogViewer()
    {
        _viewerEnabled = true;
        return this;
    }

    public LoggerOptionsBuilder WithLogLevel(LogLevel minLogLevel)
    {
        _level = minLogLevel;
        return this;
    }

    public LoggerOptions Build()
    {
        return new LoggerOptions
        {
            ViewerEnabled = _viewerEnabled,
            LogToConsole = _logToConsole,
            LogToFileOptions = _logToFileOptions,
            Level = _level,
        };
    }
}
