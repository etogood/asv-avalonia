namespace Asv.Avalonia;

public interface ICommandService
{
    IEnumerable<ICommandMetadata> Commands { get; }
    IAsyncCommand? Create(string id);
    ICommandHistory CreateHistory(IRoutableViewModel owner);
}