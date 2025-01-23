using System.Collections.Immutable;
using System.Composition;

namespace Asv.Avalonia;

public interface ICommandFactory
{
    string CommandId { get; }
    string Name { get; }
    string Description { get; }
    string Icon { get; }
    int Order { get; }
    ICommandBase Create();
}

[Export(typeof(ICommandService))]
[Shared]
public class CommandService : ICommandService
{
    private readonly ImmutableDictionary<string, ICommandFactory> _commands;

    [ImportingConstructor]
    public CommandService([ImportMany]IEnumerable<ICommandFactory> factories)
    {
        _commands = factories.ToImmutableDictionary(x => x.CommandId);
    }

    public ICommandBase? Create(string id)
    {
        return _commands.TryGetValue(id, out var command) ? command.Create() : null;
    }

    public ICommandHistory CreateHistory(string id)
    {
        var history = new CommandHistory(id, this);
        var data = File.ReadAllLines($"{id}.undo.txt");
        history.Load(data);
        return history;
    }
}