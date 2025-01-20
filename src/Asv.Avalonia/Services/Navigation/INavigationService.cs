using System.Composition;
using R3;

namespace Asv.Avalonia;

public interface INavigationService
{
    ReactiveCommand Back { get; }
    ValueTask BackwardAsync(CancellationToken cancel = default);
    ReactiveCommand Forward { get; }
    ValueTask ForwardAsync(CancellationToken cancel = default);
    ReactiveCommand GoHome { get; }
    ValueTask GoHomeAsync(CancellationToken cancel = default);
    ValueTask NavigateTo(Uri uri);
}

[Export]
public class NavigationCommandFactory : ICommandFactory
{
    public IEnumerable<ICommandMetadata> GetCommands()
    {
        throw new NotImplementedException();
    }

    public ICommandBase Create(string id)
    {
        throw new NotImplementedException();
    }
}

public class NavigationService : INavigationService
{
    
}