using ObservableCollections;

namespace Asv.Avalonia;

public class MenuNode : ObservableTreeNode<IMenuItem, string>
{
    public MenuNode(
        IMenuItem baseItem,
        IReadOnlyObservableList<IMenuItem> source,
        Func<IMenuItem, string> keySelector,
        Func<IMenuItem, string?> parentSelector,
        ObservableTreeNode<IMenuItem, string>? parentNode = null
    )
        : base(baseItem, source, keySelector, parentSelector, parentNode) { }
}
