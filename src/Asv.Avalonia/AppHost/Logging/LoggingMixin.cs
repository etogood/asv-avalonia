using Avalonia.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ZLogger;

namespace Asv.Avalonia;

public static class LoggingMixin
{
    public static IHostApplicationBuilder SetLogLevel(
        this IHostApplicationBuilder builder,
        LogLevel minLogLevel
    )
    {
        builder.Logging.SetMinimumLevel(minLogLevel);
        return builder;
    }

    public static IHostApplicationBuilder UseLogToConsole(this IHostApplicationBuilder builder)
    {
        builder.Logging.AddZLoggerConsole(options =>
        {
            options.IncludeScopes = true;
            options.OutputEncodingToUtf8 = false;
            options.UsePlainTextFormatter(formatter =>
            {
                formatter.SetPrefixFormatter(
                    $"{0:HH:mm:ss.fff} | {3:00} | ={1:short}= | {2, -40} ",
                    (in MessageTemplate template, in LogInfo info) =>
                        template.Format(
                            info.Timestamp,
                            info.LogLevel,
                            info.Category,
                            Thread.CurrentThread.ManagedThreadId
                        )
                );
            });
        });
        return builder;
    }

    public static IHostApplicationBuilder UseLogToFile(
        this IHostApplicationBuilder builder,
        string? folderName = null,
        int? rollingSizeKb = null
    )
    {
        var cfg = builder
            .Configuration.GetSection(LogServiceConfig.Section)
            .Get<LogServiceConfig>();

        var rolling =
            rollingSizeKb
            ?? cfg?.RollingSizeKb
            ?? throw new Exception($"Missing {nameof(LogServiceConfig.RollingSizeKb)} option");
        var folder =
            folderName
            ?? cfg?.Folder
            ?? throw new Exception($"Missing {nameof(LogServiceConfig.Folder)} option");

        builder.Logging.AddZLoggerRollingFile(options =>
        {
            options.FilePathSelector = (dt, index) => $"{folder}/{dt:yyyy-MM-dd}_{index}.logs";
            options.UseJsonFormatter();
            options.RollingSizeKB = rolling;
        });
        return builder;
    }

    public static IHostApplicationBuilder UseLogToConsoleOnDebug(
        this IHostApplicationBuilder builder
    )
    {
#if DEBUG
        return builder.UseLogToConsole();
#endif
        return builder;
    }
}
