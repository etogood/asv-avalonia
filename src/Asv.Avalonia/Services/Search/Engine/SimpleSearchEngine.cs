using System.Composition;

namespace Asv.Avalonia;

[Export(typeof(ISearchEngine))]
[Shared]
public class SimpleSearchEngine : ISearchEngine
{
    public const string StaticId = "Simple";
    public string Id => StaticId;
    public string Name => "Simple";
    public string Description => "A simple search engine that matches text directly without any advanced algorithms.";
    
    public void Enable()
    {
        // nothing to do here for a simple search engine
    }

    public bool Match(string text, string query)
    {
        return text.Contains(query, StringComparison.InvariantCultureIgnoreCase);
    }

    public void Disable()
    {
        // nothing to do here for a simple search engine
    }

    public IExportInfo Source => SystemModule.Instance;

    public void Dispose()
    {
        // nothing to dispose for a simple search engine
    }
}