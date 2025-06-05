using System.Collections.Immutable;
using R3;

namespace Asv.Avalonia;

public interface ISearchEngineInfo : IExportable
{
    string Id { get; }
    string Name { get; }
    string Description { get; }
}

public interface ISearchService
{
    IEnumerable<ISearchEngineInfo> Engines { get; }
    ReadOnlyReactiveProperty<ISearchEngineInfo> Engine { get; }
    void ChangeEngine(string searchEngineId);
    bool Match(string? text, string? query);
    bool Match(ISupportSearch subject, string? query) 
        => subject.GetSearchableText().Any(text => Match(text, query));
}

public interface ISupportSearch
{
    IEnumerable<string?> GetSearchableText();
}