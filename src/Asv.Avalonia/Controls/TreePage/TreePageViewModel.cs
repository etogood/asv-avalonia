using System.Collections.Immutable;
using ObservableCollections;
using R3;

namespace Asv.Avalonia;



public class TreePageViewModel<T> : PageViewModel<T>, IDesignTimeTreePage
    where T : class, IPage
{
    private IEnumerable<TreeMenuItem>? _items;
    private IDisposable? _sub1;
    private readonly IDisposable _sub2;

    public TreePageViewModel(string id, ICommandService cmd)
        : base(id, cmd)
    {
        Nodes = new ObservableList<ITreePageNode>();
        SelectedMenu = new BindableReactiveProperty<TreeMenuItem?>();
        SelectedPage = new BindableReactiveProperty<IRoutable?>();
        ObservableList<BreadCrumbItem> breadCrumbSource = [];
        BreadCrumb = breadCrumbSource.ToViewList();
        _sub2 = SelectedMenu.Subscribe(x =>
        {
            var value = x?.Base.CreateNodeViewModel();
            if (value != null)
            {
                value.Parent = this;
            }
            
            SelectedPage.Value = value;
            breadCrumbSource.Clear();
            if (SelectedMenu.Value != null)
            {
                breadCrumbSource.AddRange(SelectedMenu.Value.GetAllMenuFromRoot().Select((item, index) => new BreadCrumbItem(index == 0, item)));
            }
        });
    }

    protected override ValueTask AfterLoadExtensions()
    {
        _sub1 = Nodes.ObserveChanged().Subscribe(_ =>
        {
            // TODO: optimize update tree: only update changed nodes
            RebuildTree();
        });
        RebuildTree();
        return base.AfterLoadExtensions();
    }

    private void RebuildTree()
    {
        if (Items != null)
        {
            foreach (var item in Items)
            {
                item.Dispose();
            }
        }

        Items = Nodes.Where(x => x.ParentId == null).Select(x => new TreeMenuItem(this, x, Nodes))
            .ToImmutableArray();
    }

    public IEnumerable<TreeMenuItem>? Items
    {
        get => _items;
        private set => SetField(ref _items, value);
    }

    public BindableReactiveProperty<IRoutable?> SelectedPage { get; }
    public ISynchronizedViewList<BreadCrumbItem> BreadCrumb { get; }
    public BindableReactiveProperty<TreeMenuItem?> SelectedMenu { get; }
    public ObservableList<ITreePageNode> Nodes { get; }
    public override IEnumerable<IRoutable> Children
    {
        get
        {
            if (Items != null)
            {
                foreach (var item in Items)
                {
                    yield return item;
                }
            }

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
            _sub1?.Dispose();
            _sub2.Dispose();
            IsCompactMode.Dispose();
            SelectedMenu.Dispose();
            SelectedPage.Dispose();
            BreadCrumb.Dispose();
            if (Items != null)
            {
                foreach (var item in Items)
                {
                    item.Dispose();
                }
            }
        }

        base.Dispose(disposing);
    }
}


