using Asv.Avalonia.Tree;
using ObservableCollections;

namespace Asv.Avalonia;

public class MenuNode : ObservableTreeNode<IMenuItem, Routable.NavigationId>
{
    public MenuNode(
        IMenuItem baseItem,
        IReadOnlyObservableList<IMenuItem> source,
        Func<IMenuItem, Routable.NavigationId> keySelector,
        Func<IMenuItem, Routable.NavigationId> parentSelector,
        IComparer<IMenuItem> comparer,
        CreateNodeDelegate<IMenuItem, Routable.NavigationId> factory,
        ObservableTreeNode<IMenuItem, Routable.NavigationId>? parentNode = null
    )
        : base(baseItem, source, keySelector, parentSelector, comparer, factory, parentNode) { }
}
