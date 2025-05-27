using System.Composition;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using R3;
using ZLogger;

namespace Asv.Avalonia;

public class LogServiceConfig
{
    public const string Section = "Logs";
    public string Folder { get; set; } = "logs";
    public LogLevel Level { get; set; } = LogLevel.Information;
    public int RollingSizeKb { get; set; } = 50;
}

public class LogService : ILogService, IExportable
{
    private readonly ILoggerFactory _factory;
    private readonly ReactiveProperty<LogMessage?> _onMessage = new();
    private readonly ILogger<LogService> _logger;
    private readonly string _logsFolder;

    public LogService(IOptions<LogServiceConfig> option)
    {
        _logsFolder = option.Value.Folder;
        ArgumentNullException.ThrowIfNull(_logsFolder);
        if (!Directory.Exists(_logsFolder))
        {
            Directory.CreateDirectory(_logsFolder);
        }

        _factory = LoggerFactory.Create(builder =>
        {
            builder.ClearProviders();
            builder.SetMinimumLevel(option.Value.Level);
            builder.AddZLoggerRollingFile(options =>
            {
                options.FilePathSelector = (dt, index) =>
                    $"{_logsFolder}/{dt:yyyy-MM-dd}_{index}.logs";
                options.UseJsonFormatter();
                options.RollingSizeKB = option.Value.RollingSizeKb;
            });
        });
        _logger = _factory.CreateLogger<LogService>();
    }

    public void SaveMessage(LogMessage logMessage)
    {
        _onMessage.OnNext(logMessage);
        switch (logMessage.LogLevel)
        {
            case LogLevel.Trace:
                _logger.LogTrace(logMessage.Message);
                break;
            case LogLevel.Debug:
                _logger.LogDebug(logMessage.Message);
                break;
            case LogLevel.Information:
                _logger.LogInformation(logMessage.Message);
                break;
            case LogLevel.Warning:
                _logger.LogWarning(logMessage.Message);
                break;
            case LogLevel.Error:
                _logger.LogError(logMessage.Message);
                break;
            case LogLevel.Critical:
                _logger.LogCritical(logMessage.Message);
                break;
            case LogLevel.None:
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void DeleteLogFile()
    {
        foreach (var logFilePath in Directory.EnumerateFiles(_logsFolder, "*.logs"))
        {
            File.Delete(logFilePath);
        }
    }

    public async IAsyncEnumerable<LogMessage> LoadItemsFromLogFile(
        CancellationToken cancel = default
    )
    {
        foreach (var logFilePath in Directory.EnumerateFiles(_logsFolder, "*.logs"))
        {
            using var fs = new FileStream(
                logFilePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.ReadWrite
            );

            var rdr = new JsonTextReader(new StreamReader(fs)) { SupportMultipleContent = true };
            var serializer = new JsonSerializer();

            while (await rdr.ReadAsync(cancel))
            {
                if (rdr.TokenType == JsonToken.StartObject)
                {
                    var item = serializer.Deserialize<LogMessage>(rdr);
                    if (item != null)
                    {
                        yield return item;
                    }
                }
            }
        }
    }

    public ILogger CreateLogger(string categoryName)
    {
        return _factory.CreateLogger(categoryName);
    }

    public void AddProvider(ILoggerProvider provider)
    {
        _factory.AddProvider(provider);
    }

    public ReadOnlyReactiveProperty<LogMessage?> OnMessage => _onMessage;

    public void Dispose()
    {
        _onMessage.Dispose();
        _factory.Dispose();
    }

    public IExportInfo Source => SystemModule.Instance;
}
