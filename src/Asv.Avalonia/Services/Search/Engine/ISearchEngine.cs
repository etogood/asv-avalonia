using Avalonia.Automation;

namespace Asv.Avalonia;

public interface ISearchEngine : ISearchEngineInfo, IDisposable
{
    void Enable();
    bool Match(string text, string query);
    void Disable();
}
