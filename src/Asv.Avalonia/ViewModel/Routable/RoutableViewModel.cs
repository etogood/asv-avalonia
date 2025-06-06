namespace Asv.Avalonia.Routable;

public abstract class RoutableViewModel(NavigationId id) : DisposableViewModel(id), IRoutable
{
    private RoutedEventHandler? _routedEventHandler;

    public IRoutable? Parent { get; set; }

    public async ValueTask Rise(AsyncRoutedEvent e)
    {
        if (IsDisposed)
        {
            return;
        }

        await InternalCatchEvent(e);
        if (e.IsHandled)
        {
            return;
        }

        // If the event is handled in the current view model, try to invoke external handlers
        if (_routedEventHandler != null)
        {
            await _routedEventHandler.Invoke(this, e);
            if (e.IsHandled)
            {
                return;
            }
        }

        switch (e.RoutingStrategy)
        {
            case RoutingStrategy.Bubble:
            {
                if (Parent is not null)
                {
                    await Parent.Rise(e);
                }

                break;
            }

            case RoutingStrategy.Tunnel:
            {
                foreach (var child in GetRoutableChildren())
                {
                    await child.Rise(e);
                    if (e.IsHandled)
                    {
                        return;
                    }
                }

                break;
            }

            case RoutingStrategy.Direct:
                // Do nothing here
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public IDisposable AddEventHandler(RoutedEventHandler handler)
    {
        _routedEventHandler += handler;
        return R3.Disposable.Create(handler, RemoveEventHandler);
    }

    public void RemoveEventHandler(RoutedEventHandler handler)
    {
#pragma warning disable CS8601 // Possible null reference assignment.
        _routedEventHandler -= handler;
#pragma warning restore CS8601 // Possible null reference assignment.
    }

    public virtual ValueTask<IRoutable> Navigate(NavigationId id)
    {
        return ValueTask.FromResult(GetRoutableChildren().FirstOrDefault(x => x.Id == id) ?? this);
    }

    public abstract IEnumerable<IRoutable> GetRoutableChildren();

    protected virtual ValueTask InternalCatchEvent(AsyncRoutedEvent e)
    {
        if (e is TreeVisitorEvent visitorEvent)
        {
            visitorEvent.Visit(this);
        }
        
        return ValueTask.CompletedTask;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Parent = null;
            _routedEventHandler = null;
        }

        base.Dispose(disposing);
    }
}
