using Asv.Cfg;
using Microsoft.Extensions.Logging;
using R3;

namespace Asv.Avalonia;

public interface IAppHost : IDisposable
{
    ReadOnlyReactiveProperty<AppArgs> Args { get; }
    IAppInfo AppInfo { get; }
    IAppPath AppPath { get; }
    IConfiguration Configuration { get; }
    ILoggerFactory LoggerFactory { get; }
    void HandleApplicationCrash(Exception exception);
    bool AllowOnlyOneInstance { get; }
    bool IsFirstInstance { get; }
}
