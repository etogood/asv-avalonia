namespace Asv.Avalonia;

public interface IMemento
{
    object SaveState();
    void RestoreState(object state);
}