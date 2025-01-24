using System.Collections.Immutable;
using System.Composition;

namespace Asv.Avalonia;

public interface ICommandMetadata
{
    string CommandId { get; }
    string Name { get; }
    string Description { get; }
    string Icon { get; }
    int Order { get; }
}

public interface ICommandFactory
{
    string CommandId { get; }
    string Name { get; }
    string Description { get; }
    string Icon { get; }
    int Order { get; }
    IAsyncCommand Create();
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

    public IEnumerable<ICommandMetadata> Commands { get; }

    public IAsyncCommand? Create(string id)
    {
        return _commands.TryGetValue(id, out var command) ? command.Create() : null;
    }

    public ICommandHistory CreateHistory(IRoutableViewModel owner)
    {
        var history = new CommandHistory(owner, this);
        return history;
    }
}