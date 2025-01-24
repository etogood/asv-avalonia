using System.Collections.Immutable;
using ObservableCollections;
using R3;

namespace Asv.Avalonia;



public class TreePageViewModel<T> : PageViewModel<T>, IDesignTimeTreePage
    where T : class, IPage
{
    private IEnumerable<TreeMenuItem>? _items;
    private readonly IDisposable _sub5;

    public TreePageViewModel(string id, ICommandService cmd, params IEnumerable<IExtensionFor<T>> extensions)
        : base(id, cmd, extensions)
    {
        Nodes = new();
        _sub5 = Nodes.ObserveChanged().Subscribe(_ => RebuildTree());
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

    public ObservableList<ITreePageNode> Nodes { get; }
    public override IEnumerable<IRoutable> Children => Items ?? [];
    public BindableReactiveProperty<bool> IsCompactMode { get; } = new();

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _sub5.Dispose();
            IsCompactMode.Dispose();
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
