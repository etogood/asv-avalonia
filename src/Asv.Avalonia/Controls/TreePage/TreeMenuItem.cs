using System.Collections.Immutable;
using R3;

namespace Asv.Avalonia;

public class TreeMenuItem : RoutableViewModel
{
    public TreeMenuItem(IRoutable routeParent, ITreePageNode node, IReadOnlyList<ITreePageNode> items)
        : base(node.Id)
    {
        Base = node;
        Parent = routeParent;
        Items = items.Where(x => x.ParentId == node.Id).Select(x => new TreeMenuItem(routeParent, x, items)
        {
            Parent = routeParent,
        }).ToImmutableArray();
    }

    public IEnumerable<TreeMenuItem> Items { get; }
    public ITreePageNode Base { get; }

    public BindableReactiveProperty<bool> IsExpanded { get; } = new();
    protected override ValueTask InternalCatchEvent(AsyncRoutedEvent e)
    {
        return ValueTask.CompletedTask;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            foreach (var item in Items)
            {
                item.Dispose();
            }

            IsExpanded.Dispose();
        }

        base.Dispose(disposing);
    }
}