namespace Asv.Avalonia;

public interface ICommandService
{
    ICommandBase Create(string id);
    ICommandHistory CreateHistory(string id);
}