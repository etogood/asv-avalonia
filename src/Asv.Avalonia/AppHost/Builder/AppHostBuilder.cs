using System.Reflection;
using Asv.Cfg;
using Asv.Cfg.Json;
using Avalonia;
using Microsoft.Extensions.Logging;
using ZLogger;

namespace Asv.Avalonia;

internal class AppHostBuilder : IAppHostBuilder
{
    private const string ZeroVersion = "0.0.0";
    private Func<IConfiguration> _createConfigCallback;
    private string _appName = string.Empty;
    private string _appVersion = ZeroVersion;
    private string _companyName = string.Empty;
    private string _avaloniaVersion = ZeroVersion;
    private Func<IConfiguration, LogLevel> _setMinLevelCallback;
    private readonly List<Action<IConfiguration, ILoggingBuilder>> _logBuilderCallbacks = new();
    private AppArgs _args = new([]);
    private Func<IConfiguration, IAppInfo, string> _userDataFolder;
    private string _productTitle = string.Empty;
    private readonly string _appFolder;
    private Func<IAppInfo, string?> _mutexName;
    private Func<IAppInfo, string?> _namedPipe;

    public AppHostBuilder()
    {
        _userDataFolder = (_, info) =>
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                info.Name
            );
        _appFolder =
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
        _createConfigCallback = () => new JsonOneFileConfiguration("config.json", true, null);
        _setMinLevelCallback = _ => LogLevel.Information;
        _mutexName = _ => null;
        _namedPipe = _ => null;
        WithAppInfoFrom(Assembly.GetExecutingAssembly());
        WithAvaloniaVersion(
            typeof(AppBuilder).Assembly.GetName().Version?.ToString() ?? ZeroVersion
        );
        WithLogMinimumLevel(LogLevel.Information);
    }

    internal IAppHost Create()
    {
        var config = _createConfigCallback();
        var appInfo = new AppInfo
        {
            Name = _appName,
            Version = _appVersion,
            CompanyName = _companyName,
            AvaloniaVersion = _avaloniaVersion,
            Title = _productTitle,
        };
        var appPath = new AppPath
        {
            UserDataFolder = _userDataFolder(config, appInfo),
            AppFolder = _appFolder,
        };
        var logFactory = LoggerFactory.Create(builder =>
        {
            builder.ClearProviders();
            builder.SetMinimumLevel(_setMinLevelCallback(config));
            foreach (var logBuilderCallback in _logBuilderCallbacks)
            {
                logBuilderCallback(config, builder);
            }
        });
        return new AppHost(
            config,
            appPath,
            appInfo,
            logFactory,
            _args,
            _mutexName(appInfo),
            _namedPipe(appInfo)
        );
    }

    #region SingleInstance

    public IAppHostBuilder EnforceSingleInstance(string? mutexName = null)
    {
        _mutexName = _ => mutexName ?? _.Name;
        return this;
    }

    public IAppHostBuilder EnableArgumentForwarding(string? namedPipeName = null)
    {
        _namedPipe = _ => namedPipeName ?? _.Name;
        return this;
    }

    #endregion

    #region Logging

    public IAppHostBuilder AddLog(Action<IConfiguration, ILoggingBuilder> logBuilderCallback)
    {
        _logBuilderCallbacks.Add(logBuilderCallback);
        return this;
    }

    public IAppHostBuilder WithLogMinimumLevel(LogLevel minLogLevel)
    {
        _setMinLevelCallback = _ => minLogLevel;
        return this;
    }

    public IAppHostBuilder WithLogMinimumLevel<TConfig>(Func<TConfig, LogLevel> fromConfig)
        where TConfig : new()
    {
        _setMinLevelCallback = x => fromConfig(x.Get<TConfig>());
        return this;
    }

    public IAppHostBuilder AddLogToJson<TConfig>(
        Func<TConfig, string> logFolder,
        Func<TConfig, int> rollingSizeKb
    )
        where TConfig : new()
    {
        _logBuilderCallbacks.Add(
            (cfg, builder) =>
            {
                var config = cfg.Get<TConfig>();
                var logsFolder = logFolder(config);
                var size = rollingSizeKb(config);
                builder.AddZLoggerRollingFile(options =>
                {
                    options.FilePathSelector = (dt, index) =>
                        $"{logsFolder}/{dt:yyyy-MM-dd}_{index}.logs";
                    options.UseJsonFormatter();
                    options.RollingSizeKB = size;
                });
            }
        );
        return this;
    }

    public IAppHostBuilder AddLogToJson<TConfig>(string logFolder, Func<TConfig, int> rollingSizeKb)
        where TConfig : new()
    {
        _logBuilderCallbacks.Add(
            (cfg, builder) =>
            {
                var config = cfg.Get<TConfig>();
                var size = rollingSizeKb(config);
                builder.AddZLoggerRollingFile(options =>
                {
                    options.FilePathSelector = (dt, index) =>
                        $"{logFolder}/{dt:yyyy-MM-dd}_{index}.logs";
                    options.UseJsonFormatter();
                    options.RollingSizeKB = size;
                });
            }
        );
        return this;
    }

    public IAppHostBuilder AddLogToJson(string logFolder, int rollingSizeKb)
    {
        _logBuilderCallbacks.Add(
            (cfg, builder) =>
            {
                builder.AddZLoggerRollingFile(options =>
                {
                    options.FilePathSelector = (dt, index) =>
                        $"{logFolder}/{dt:yyyy-MM-dd}_{index}.logs";
                    options.UseJsonFormatter();
                    options.RollingSizeKB = rollingSizeKb;
                });
            }
        );
        return this;
    }

    public IAppHostBuilder AddLogToConsole()
    {
        _logBuilderCallbacks.Add(
            (cfg, builder) =>
            {
                builder.AddZLoggerConsole(options =>
                {
                    options.IncludeScopes = true;
                    options.OutputEncodingToUtf8 = false;
                    options.UsePlainTextFormatter(formatter =>
                    {
                        formatter.SetPrefixFormatter(
                            $"{0:HH:mm:ss.fff} | ={1:short}= | {2, -40} ",
                            (in MessageTemplate template, in LogInfo info) =>
                                template.Format(info.Timestamp, info.LogLevel, info.Category)
                        );

                        // formatter.SetExceptionFormatter((writer, ex) => Utf8StringInterpolation.Utf8String.Format(writer, $"{ex.Message}"));
                    });
                });
            }
        );
        return this;
    }

    public IAppHostBuilder WithArguments(string[] args)
    {
        _args = new AppArgs(args);
        return this;
    }

    public IAppHostBuilder WithUserDataFolder(string userFolder)
    {
        _userDataFolder = (_, _) => userFolder;
        return this;
    }

    #endregion

    #region Configuration

    public IAppHostBuilder WithConfiguration(IConfiguration configuration)
    {
        _createConfigCallback = () => configuration;
        return this;
    }

    public IAppHostBuilder WithJsonConfiguration(
        string fileName,
        bool createIfNotExist,
        TimeSpan? flushToFileDelayMs
    )
    {
        _createConfigCallback = () =>
            new JsonOneFileConfiguration(fileName, createIfNotExist, flushToFileDelayMs);
        return this;
    }

    public IAppHostBuilder WithAppInfoFrom(Assembly assembly)
    {
        WithProductName(assembly);
        WithVersion(assembly);
        WithCompanyName(assembly);
        WithProductTitle(assembly);
        return this;
    }

    #endregion

    #region ProductTitle

    public IAppHostBuilder WithProductTitle(string productTitle)
    {
        _productTitle = productTitle;
        return this;
    }

    public IAppHostBuilder WithProductTitle(Assembly assembly)
    {
        _productTitle = GetTitle(assembly);
        return this;
    }

    private static string GetTitle(Assembly src)
    {
        var attributes = src.GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
        if (attributes.Length > 0)
        {
            var titleAttribute = (AssemblyTitleAttribute)attributes[0];
            if (titleAttribute.Title.Length > 0)
            {
                return titleAttribute.Title;
            }
        }

        return src.GetName().Name ?? string.Empty;
    }

    #endregion

    #region AvaloniaVersion

    public IAppHostBuilder WithAvaloniaVersion(string avaloniaVersion)
    {
        _avaloniaVersion = avaloniaVersion;
        return this;
    }

    #endregion

    #region AppName

    public IAppHostBuilder WithProductName(string appName)
    {
        _appName = appName;
        return this;
    }

    public IAppHostBuilder WithProductName(Assembly assembly)
    {
        _appName = GetAppName(assembly);
        return this;
    }

    private static string GetAppName(Assembly src)
    {
        var attributes = src.GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
        if (attributes.Length == 0)
        {
            return src.GetName().Name ?? string.Empty;
        }

        var titleAttribute = (AssemblyTitleAttribute)attributes[0];
        return titleAttribute.Title.Length > 0
            ? titleAttribute.Title
            : src.GetName().Name ?? string.Empty;
    }

    #endregion

    #region Version

    public IAppHostBuilder WithVersion(string version)
    {
        _appVersion = version;
        return this;
    }

    public IAppHostBuilder WithVersion(Assembly assembly)
    {
        _appVersion = GetInformationalVersion(assembly);
        return this;
    }

    private static string GetInformationalVersion(Assembly src)
    {
        var attributes = src.GetCustomAttributes(
            typeof(AssemblyInformationalVersionAttribute),
            false
        );
        return attributes.Length == 0
            ? ZeroVersion
            : ((AssemblyInformationalVersionAttribute)attributes[0]).InformationalVersion;
    }

    #endregion

    #region CompanyName

    public IAppHostBuilder WithCompanyName(string companyName)
    {
        _companyName = companyName;
        return this;
    }

    public IAppHostBuilder WithCompanyName(Assembly assembly)
    {
        _companyName = GetCompanyName(assembly);
        return this;
    }

    private static string GetCompanyName(Assembly src)
    {
        var attributes = src.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
        return attributes.Length == 0
            ? string.Empty
            : ((AssemblyCompanyAttribute)attributes[0]).Company;
    }

    #endregion
}
