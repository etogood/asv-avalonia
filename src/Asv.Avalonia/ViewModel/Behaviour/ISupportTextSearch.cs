namespace Asv.Avalonia;

public interface ISupportTextSearch : IRoutable
{
    string SearchText { get; }
    void Query(string text);
    void Focus();
}
