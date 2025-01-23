namespace Asv.Avalonia;

public abstract class RoutableViewModel(string id) : DisposableViewModel(id), IRoutableViewModel
{
    public IRoutableViewModel? Parent { get; set; }
    public abstract IEnumerable<IRoutableViewModel> Children { get; }

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

        switch (e.RoutingEventStrategy)
        {
            case RoutingEventStrategy.Bubble when Parent is not null:
                await Parent.Rise(e);
                break;
            case RoutingEventStrategy.Tunnel:
            {
                foreach (var child in Children)
                {
                    await child.Rise(e);
                    if (e.IsHandled)
                    {
                        break;
                    }
                }

                return;
            }

            case RoutingEventStrategy.Direct:
                return;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    protected abstract ValueTask InternalCatchEvent(AsyncRoutedEvent e);
}