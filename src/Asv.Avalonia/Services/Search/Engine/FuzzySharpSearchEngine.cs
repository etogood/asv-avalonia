using System.Composition;
using FuzzySharp;

namespace Asv.Avalonia;

[Export(typeof(ISearchEngine))]
[Shared]
public class FuzzySharpSearchEngine : ISearchEngine
{
    private const int DefaultRatio = 50;
    public const string StaticId = "FuzzySharp";
    public IExportInfo Source => SystemModule.Instance;
    public string Id => StaticId;
    public string Name => "FuzzySharp";
    public string Description => "A search engine that uses FuzzySharp for fuzzy matching of text.";
    public void Dispose()
    {
        // nothing to dispose for a simple search engine
    }

    public void Enable()
    {
        // nothing to do here for a simple search engine
    }

    public bool Match(string text, string query)
    {
        if (Fuzz.PartialRatio(text, query) > DefaultRatio)
        {
            return true;
        }

        return false;
    }

    public void Disable()
    {
        // nothing to do here for a simple search engine
    }
}