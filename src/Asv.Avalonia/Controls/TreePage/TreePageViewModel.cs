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
        Nodes = new();
        SelectedMenu = new();
        SelectedPage = new();
        _sub2 = SelectedMenu.Subscribe(x =>
        {
            SelectedPage.Value = x?.Base.CreateNodeViewModel();
        });
    }

    protected override ValueTask AfterLoadExtensions()
    {
        _sub1 = Nodes.ObserveChanged().Subscribe(_ =>
        {
            // TODO: update tree
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
    public BindableReactiveProperty<TreeMenuItem?> SelectedMenu { get; }
    public ObservableList<ITreePageNode> Nodes { get; }
    public override IEnumerable<IRoutable> Children => Items ?? [];
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
