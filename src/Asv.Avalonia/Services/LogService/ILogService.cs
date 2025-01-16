using Microsoft.Extensions.Logging;

namespace Asv.Avalonia;

#pragma warning disable SA1106
public interface ILogService : ILoggerFactory;
#pragma warning restore SA1106

public class LogService : ILogService
{
    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public ILogger CreateLogger(string categoryName)
    {
        throw new NotImplementedException();
    }

    public void AddProvider(ILoggerProvider provider)
    {
        throw new NotImplementedException();
    }
}
