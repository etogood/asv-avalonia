using System.Reflection;
using Asv.Cfg;
using Microsoft.Extensions.Logging;

namespace Asv.Avalonia;

public interface IAppHostBuilder
{
    IAppHostBuilder WithConfiguration(IConfiguration configuration);
    IAppHostBuilder WithJsonConfiguration(
        string fileName,
        bool createIfNotExist,
        TimeSpan? flushToFileDelayMs
    );
    IAppHostBuilder WithAppInfoFrom(Assembly assembly);
    IAppHostBuilder WithProductName(string appName);
    IAppHostBuilder WithProductName(Assembly assembly);
    IAppHostBuilder WithProductTitle(string productTitle);
    IAppHostBuilder WithProductTitle(Assembly assembly);
    IAppHostBuilder WithVersion(string version);
    IAppHostBuilder WithVersion(Assembly assembly);
    IAppHostBuilder WithCompanyName(string companyName);
    IAppHostBuilder WithCompanyName(Assembly assembly);
    IAppHostBuilder WithAvaloniaVersion(string avaloniaVersion);

    IAppHostBuilder WithLogMinimumLevel(LogLevel logLevel);
    IAppHostBuilder WithLogMinimumLevel<TConfig>(Func<TConfig, LogLevel> fromConfig)
        where TConfig : new();
    IAppHostBuilder AddLog(Action<IConfiguration, ILoggingBuilder> logBuilderCallback);
    IAppHostBuilder AddLogToJson<TConfig>(
        Func<TConfig, string> logFolder,
        Func<TConfig, int> rollingSizeKb
    )
        where TConfig : new();
    IAppHostBuilder AddLogToJson<TConfig>(string logFolder, Func<TConfig, int> rollingSizeKb)
        where TConfig : new();
    IAppHostBuilder AddLogToJson(string logFolder, int rollingSizeKb);
    IAppHostBuilder AddLogToConsole();

    IAppHostBuilder WithArguments(string[] args);
    IAppHostBuilder WithUserDataFolder(string userFolder);

    /// <summary>
    /// Ensures that only a single instance of the application is running on the system.
    /// If another instance is already running, the current instance will not proceed further.
    /// </summary>
    /// <returns></returns>
    IAppHostBuilder EnforceSingleInstance(string? mutexName = null);

    /// <summary>
    /// Enables forwarding of command-line arguments to an already running instance of the application.
    /// If the application is not already running, the current instance will handle the arguments as usual.
    /// Must be called with <see cref="EnforceSingleInstance"/>.
    /// </summary>
    /// <returns></returns>
    IAppHostBuilder EnableArgumentForwarding(string? namedPipeName = null);
}
