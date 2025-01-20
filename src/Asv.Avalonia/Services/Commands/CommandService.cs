namespace Asv.Avalonia;

public class CommandService : ICommandService
{
    public ICommandHistory CreateHistory(string id)
    {
        return new CommandHistory(id);
    }
}
