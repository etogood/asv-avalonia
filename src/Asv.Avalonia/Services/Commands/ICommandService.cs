namespace Asv.Avalonia;

public interface ICommandService
{
    ICommandHistory CreateHistory(string id);
}