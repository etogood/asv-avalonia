using System.Collections.Immutable;
using R3;

namespace Asv.Avalonia;

public abstract class RoutableViewModel(string id) : DisposableViewModel(id), IRoutable
{
    public IRoutable? NavigationParent { get; set; }
    public virtual IEnumerable<IRoutable> NavigationChildren => [];

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
            case RoutingEventStrategy.Bubble:
                if (NavigationParent is not null)
                {
                    await NavigationParent.Rise(e);
                }

                break;
            case RoutingEventStrategy.Tunnel:
            {
                foreach (var child in NavigationChildren)
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
                throw new ArgumentOutOfRangeException($"Unknown {nameof(RoutingEventStrategy)} value: {e.RoutingEventStrategy}");
        }
    }

    public async ValueTask<IRoutable> NavigateTo(ArraySegment<string> path)
    {
        IsFocused.Value = true;

        if (path.Count == 0)
        {
            await Rise(new NavigationEvent(this));
            return this;
        }

        foreach (var child in NavigationChildren)
        {
            if (child.Id == path[0])
            {
                return await child.NavigateTo(path[1..]);
            }
        }

        return await NavigateToUnknownPath(path);
    }

    public BindableReactiveProperty<bool> IsFocused { get; } = new(false);

    protected virtual ValueTask<IRoutable> NavigateToUnknownPath(ArraySegment<string> path)
    {
        return ValueTask.FromException<IRoutable>(new NavigationException($"Can't find child with id {path[0]}"));
    }

    protected virtual ValueTask InternalCatchEvent(AsyncRoutedEvent e) => ValueTask.CompletedTask;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            IsFocused.Dispose();
        }

        base.Dispose(disposing);
    }
}