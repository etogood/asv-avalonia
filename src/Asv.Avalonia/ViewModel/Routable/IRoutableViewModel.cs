namespace Asv.Avalonia;

public interface IRoutableViewModel : IViewModel
{
    IRoutableViewModel? Parent { get; set; }
    IEnumerable<IRoutableViewModel> Children { get; }
    ValueTask Rise(AsyncRoutedEvent e);
}

public enum RoutingEventStrategy
{
    Bubble,
    Tunnel,
    Direct,
}

public abstract class AsyncRoutedEvent(IRoutableViewModel source, RoutingEventStrategy routingEventStrategy)
{
    public IRoutableViewModel Source { get; } = source;
    public RoutingEventStrategy RoutingEventStrategy { get; } = routingEventStrategy;
    public bool IsHandled { get; set; }
    public virtual AsyncRoutedEvent Clone()
    {
        return (AsyncRoutedEvent)MemberwiseClone();
    }
}