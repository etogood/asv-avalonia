using ObservableCollections;

namespace Asv.Avalonia;

public class MenuCollection : ObservableTree<IMenuItem, string>
{
    public MenuCollection(
        IReadOnlyObservableList<IMenuItem> source,
        Func<IMenuItem, string> keySelector,
        Func<IMenuItem, string?> parentSelector
    )
        : base(
            source,
            keySelector,
            parentSelector,
            (item, list, selector, func) => new MenuNode(item, list, selector, func)
        ) { }
}
