namespace Asv.Avalonia;

public abstract class TreeVisitorEvent(IRoutable source)
    : AsyncRoutedEvent(source, RoutingStrategy.Tunnel)
{
    public abstract void Visit(IRoutable source);

    public static async ValueTask<IReadOnlyList<TViewModel>> VisitAll<TViewModel>(IRoutable context)
        where TViewModel : IRoutable
    {
        var root = context.GetRoot();
        var items = new List<TViewModel>();
        var ev = new TreeVisitorEvent<TViewModel>(root, items.Add);
        await root.Rise(ev);
        return items;
    }
}

public class TreeVisitorEvent<TViewModel>(IRoutable source, Action<TViewModel> visitor)
    : TreeVisitorEvent(source)
    where TViewModel : IRoutable
{
    public override void Visit(IRoutable source)
    {
        if (source is TViewModel target)
        {
            visitor(target);
        }
    }
}
