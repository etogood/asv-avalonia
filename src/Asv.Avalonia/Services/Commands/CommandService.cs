using System.Composition;

namespace Asv.Avalonia;

public interface ICommandMetadata
{
    string Id { get; }
    string Name { get; }
    string Description { get; }
    string Icon { get; }
    int Order { get; }
}

public interface ICommandFactory
{
    IEnumerable<ICommandMetadata> GetCommands();
    ICommandBase Create(string id);
}

public class CommandService : ICommandService
{
    IDictionary<string, Func<>>
    [ImportingConstructor]
    public CommandService(IEnumerable<ICommandFactory> factories)
    {
        
    }
    
    public ICommandBase Create(string id)
    {
                        
    }
    
    public ICommandHistory CreateHistory(string id)
    {
        var history = new CommandHistory(id);
        var data = File.ReadAllLines($"{id}.undo.txt");
        history.Load(data);
        return history;
    }
}