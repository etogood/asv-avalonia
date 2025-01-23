namespace Asv.Avalonia;

public interface IOriginator
{
    IMemento Save();
    void Restore(IMemento state);
}