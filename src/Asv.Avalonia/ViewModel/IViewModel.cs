using R3;

namespace Asv.Avalonia;

public interface IViewModel : IDisposable
{
    string Id { get; }
}

public interface ISupportEvents : IViewModel
{
    ISupportEvents? Parent { get; }
    IEnumerable<ISupportEvents> Children { get; }
    IEventManager Events { get; }
}

public interface IEventManager
{
    Observable<ViewModelEvent> OnEvent { get; }
    ValueTask Rise(ViewModelEvent e);
}

public class EventManager(IViewModel viewModel) : IEventManager
{
    private Subject<ViewModelEvent>? _onEvent;
    public Observable<ViewModelEvent> OnEvent => _onEvent ??= new Subject<ViewModelEvent>();
    public ValueTask Rise(ViewModelEvent e)
    {
        _onEvent?.OnNext(e);
        if (e.IsHandled)
        {
            return ValueTask.CompletedTask;
        }

        if (e.RoutingStrategy == RoutingStrategy.Bubble)
        {
            if (viewModel.Parent is not null)
            {
                return viewModel.Parent.Events.Rise(e);
            }
        }
        else if (e.RoutingStrategy == RoutingStrategy.Tunnel)
        {
            foreach (var child in viewModel.Children)
            {
                return child.Events.Rise(e);
            }
        }

        return ValueTask.CompletedTask;
    }
}

public abstract class ViewModelEvent(IViewModel source, RoutingStrategy routingStrategy)
{
    public IViewModel Source { get; } = source;
    public RoutingStrategy RoutingStrategy { get; } = routingStrategy;
    public bool IsHandled { get; set; }
    public virtual ViewModelEvent Clone()
    {
        return (ViewModelEvent)MemberwiseClone();
    }
}

public enum RoutingStrategy
{
    Bubble,
    Tunnel,
}
