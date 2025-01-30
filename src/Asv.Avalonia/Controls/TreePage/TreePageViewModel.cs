using System.Collections.Immutable;
using System.Composition;
using System.Composition.Hosting;
using ObservableCollections;
using R3;

namespace Asv.Avalonia;



public class TreePageViewModel<TContext> : PageViewModel<TContext>, IDesignTimeTreePage
    where TContext : class, IPage
{
    private readonly IContainerHost _container;
    private readonly IDisposable _sub2;
    private readonly ObservableList<BreadCrumbItem> _breadCrumbSource;
    private bool _internalNavigate;

    public TreePageViewModel(string id, ICommandService cmd, IContainerHost container)
        : base(id, cmd)
    {
        _container = container;
        Nodes = new ObservableList<ITreePageNode>();
        SelectedNode = new BindableReactiveProperty<ObservableTreeNode<ITreePageNode, string>?>();
        SelectedPage = new BindableReactiveProperty<IRoutable?>();
        _breadCrumbSource = new ObservableList<BreadCrumbItem>();
        BreadCrumb = _breadCrumbSource.ToViewList();
        Tree = Nodes.ToObservableTree(x => x.Id, x => x.ParentId);
        _sub2 = SelectedNode.SubscribeAwait(async (x, _) =>
        {
            if (x?.Base.NavigateTo == null || _internalNavigate)
            {
                return;
            }

            _breadCrumbSource.Clear();
            if (SelectedNode.Value != null)
            {
                _breadCrumbSource.AddRange(SelectedNode.Value.GetAllMenuFromRoot().Select((item, index) => new BreadCrumbItem(index == 0, item.Base)));
            }

            await NavigateTo(new ArraySegment<string>([x.Base.NavigateTo]));
        });
    }

    protected override ValueTask<IRoutable> NavigateToUnknownPath(ArraySegment<string> path)
    {
        if (path.Count == 0)
        {
            throw new ArgumentException("Value cannot be an empty collection.", nameof(path));
        }

        var first = path[0];
        if (SelectedNode.Value?.Base.NavigateTo != first)
        {
            _internalNavigate = true;
            SelectedNode.Value = Tree.FindNode(x => x.Base.NavigateTo == first);
            _internalNavigate = false;
        }

        var newPage = CreateSubPage(first);
        if (newPage == null)
        {
            return base.NavigateToUnknownPath(path);
        }

        SelectedPage.Value?.Dispose();
        newPage.NavigationParent = this;
        SelectedPage.Value = newPage;
        return newPage.NavigateTo(path[1..]);
    }

    protected virtual ISettingsSubPage? CreateSubPage(string id)
    {
        return _container.TryGetExport<ISettingsSubPage>(id, out var page) ? page : null;
    }

    public ObservableTree<ITreePageNode, string> Tree { get; }
    public BindableReactiveProperty<IRoutable?> SelectedPage { get; }
    public ISynchronizedViewList<BreadCrumbItem> BreadCrumb { get; }
    public BindableReactiveProperty<ObservableTreeNode<ITreePageNode, string>?> SelectedNode { get; }
    public ObservableList<ITreePageNode> Nodes { get; }
    public override IEnumerable<IRoutable> NavigationChildren
    {
        get
        {
            if (SelectedPage.Value != null)
            {
                yield return SelectedPage.Value;
            }
        }
    }

    public BindableReactiveProperty<bool> IsCompactMode { get; } = new();

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _sub2.Dispose();
            IsCompactMode.Dispose();
            SelectedNode.Dispose();
            SelectedPage.Dispose();
            BreadCrumb.Dispose();
            Tree.Dispose();
        }

        base.Dispose(disposing);
    }
}


