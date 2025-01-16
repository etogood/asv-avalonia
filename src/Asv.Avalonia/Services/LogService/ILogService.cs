using Microsoft.Extensions.Logging;

namespace Asv.Avalonia;

public interface ILogService : ILoggerFactory
{
    
}

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