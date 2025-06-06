using Asv.Avalonia.Tree;
using ObservableCollections;

namespace Asv.Avalonia;

public class TreePageMenuNode : ObservableTreeNode<ITreePage, Routable.NavigationId>
{
    public TreePageMenuNode(
        ITreePage baseItem,
        IReadOnlyObservableList<ITreePage> source,
        Func<ITreePage, Routable.NavigationId> keySelector,
        Func<ITreePage, Routable.NavigationId> parentSelector,
        IComparer<ITreePage> comparer,
        CreateNodeDelegate<ITreePage, Routable.NavigationId> createNodeFactory,
        ObservableTreeNode<ITreePage, Routable.NavigationId>? parentNode
    )
        : base(
            baseItem,
            source,
            keySelector,
            parentSelector,
            comparer,
            createNodeFactory,
            parentNode
        ) { }
}
