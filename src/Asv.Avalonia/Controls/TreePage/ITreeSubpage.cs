using Asv.Common;
using ObservableCollections;

namespace Asv.Avalonia;

public interface ITreeSubpage : IRoutable, IExportable
{
    MenuTree MenuView { get; }
    ObservableList<IMenuItem> Menu { get; }
}

public interface ITreeSubpage<in TContext> : ITreeSubpage
    where TContext : class, IPage
{
    ValueTask Init(TContext context);
}

public abstract class TreeSubpage<TContext> : RoutableViewModel, ITreeSubpage<TContext>
    where TContext : class, IPage
{
    protected TreeSubpage(NavigationId id)
        : base(id)
    {
        Menu.SetRoutableParent(this, true).DisposeItWith(Disposable);
        MenuView = new MenuTree(Menu).DisposeItWith(Disposable);
    }

    public abstract ValueTask Init(TContext context);

    public override IEnumerable<IRoutable> GetRoutableChildren() => Menu;

    public abstract IExportInfo Source { get; }
    public MenuTree MenuView { get; }
    public ObservableList<IMenuItem> Menu { get; } = [];

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Menu.Clear();
        }

        base.Dispose(disposing);
    }
}
