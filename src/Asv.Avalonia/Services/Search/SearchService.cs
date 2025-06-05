using System.Collections.Immutable;
using System.Composition;
using Asv.Cfg;
using Asv.Common;
using R3;

namespace Asv.Avalonia;

public class SearchServiceConfig
{
    public string SearchEngine { get; set; } = FuzzySharpSearchEngine.StaticId;
}

[Export(typeof(ISearchService))]
[Shared]
public class SearchService : AsyncDisposableOnce, ISearchService
{
    private readonly IConfiguration _config;
    private readonly ImmutableArray<ISearchEngine> _engines;
    private readonly ReaderWriterLockSlim _lock = new();
    private ISearchEngine _currentEngine;

    [ImportingConstructor]
    public SearchService(
        IConfiguration config,
        [ImportMany] IEnumerable<ISearchEngine> searchEngines
    )
    {
        _config = config;
        _engines = [.. searchEngines];
        var searchEngineId = config.Get<SearchServiceConfig>().SearchEngine;
        _currentEngine = _engines.FirstOrDefault(e => e.Id == searchEngineId) ?? _engines[0];
    }

    public IEnumerable<ISearchEngineInfo> Engines => _engines;
    ReadOnlyReactiveProperty<ISearchEngineInfo> ISearchService.Engine => Engine;
    public ReactiveProperty<ISearchEngineInfo> Engine { get; } = new();

    public void ChangeEngine(string searchEngineId)
    {
        var newEngine = _engines.FirstOrDefault(e => e.Id == searchEngineId) ?? _engines[0];
        if (newEngine == _currentEngine)
        {
            return;
        }

        _lock.EnterWriteLock();
        try
        {
            _currentEngine.Disable();
            _currentEngine = newEngine;
            _currentEngine.Enable();
            Engine.Value = _currentEngine;
            _config.Set(new SearchServiceConfig { SearchEngine = _currentEngine.Id });
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    public bool Match(string? text, string? query)
    {
        _lock.EnterReadLock();
        try
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return true;
            }

            if (string.IsNullOrWhiteSpace(text))
            {
                return false;
            }

            if (_currentEngine == null)
            {
                throw new InvalidOperationException("Search engine is not set.");
            }

            return _currentEngine.Match(text, query);
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    #region Dispose

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _lock.Dispose();
            Engine.Dispose();
        }

        base.Dispose(disposing);
    }

    protected override async ValueTask DisposeAsyncCore()
    {
        await CastAndDispose(_lock);
        await CastAndDispose(Engine);

        await base.DisposeAsyncCore();

        return;

        static async ValueTask CastAndDispose(IDisposable resource)
        {
            if (resource is IAsyncDisposable resourceAsyncDisposable)
            {
                await resourceAsyncDisposable.DisposeAsync();
            }
            else
            {
                resource.Dispose();
            }
        }
    }

    #endregion
}
