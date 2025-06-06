using Asv.Avalonia.Tree;
using ObservableCollections;

namespace Asv.Avalonia;

public class MenuTree : ObservableTree<IMenuItem, Routable.NavigationId>
{
    public MenuTree(IReadOnlyObservableList<IMenuItem> flatList)
        : base(
            flatList,
            Routable.NavigationId.Empty,
            x => x.Id,
            x => x.ParentId,
            MenuComparer.Instance,
            (item, list, key, parent, comparer, transform, node) =>
                new MenuNode(item, list, key, parent, comparer, transform, node)
        ) { }
}
