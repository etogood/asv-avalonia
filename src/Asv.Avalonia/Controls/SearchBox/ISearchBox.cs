namespace Asv.Avalonia;

public interface ISearchBox : IRoutable
{
    string SearchText { get; }
    void Query(string text);
    void Refresh();
}
