using System.Collections.Immutable;
using System.Composition;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using R3;
using ZLogger;

namespace Asv.Avalonia;

public class ExtendableViewModel<TSelfInterface> : RoutableViewModel
    where TSelfInterface : class
{
    protected ExtendableViewModel(string id)
        : base(id)
    {
    }

    private TSelfInterface GetContext() => this as TSelfInterface ??
                                        throw new Exception($"Class {GetType().FullName} must implement {nameof(TSelfInterface)}");

    [ImportMany]
    public IEnumerable<Lazy<IExtensionFor<TSelfInterface>>>? Extensions { get; set; }
    [Import]
    public ILoggerFactory? LoggerFactory { get; set; }

    [OnImportsSatisfied]
    public async void Load()
    {
        try
        {
            if (Extensions == null)
            {
                return;
            }

            var logger = LoggerFactory?.CreateLogger(GetType()) ?? NullLogger.Instance;

            var context = GetContext();
            foreach (var extension in Extensions)
            {
                try
                {
                    await extension.Value.Extend(context);
                }
                catch (Exception e)
                {
                    logger.ZLogError(e, $"Error while loading extension {extension.Value.GetType().FullName} for {GetType().FullName}");
                }
            }

            await AfterLoadExtensions();
        }
        catch (Exception e)
        {
            LoggerFactory?.CreateLogger(GetType()).ZLogError(e, $"Error while load extensions for {GetType().FullName}");
        }
    }

    protected virtual ValueTask AfterLoadExtensions()
    {
        return ValueTask.CompletedTask;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (Extensions != null)
            {
                foreach (var item in Extensions)
                {
                    if (item.IsValueCreated)
                    {
                        item.Value.Dispose();
                    }
                }
            }
        }

        base.Dispose(disposing);
    }
}