using R3;

namespace Asv.Avalonia;

public interface IDesignTimeTreePage : IPage
{
    BindableReactiveProperty<bool> IsCompactMode { get; }
    IEnumerable<TreeMenuItem>? Items { get; }
}

public class DesignTimeTreePage : TreePageViewModel<IPage>
{
    public DesignTimeTreePage()
        : base(DesignTime.Id, DesignTime.CommandService)
    {
        DesignTime.ThrowIfNotDesignMode();
        Nodes.Add(new TreePageNode("node1", () => null!));
        Nodes.Add(new TreePageNode("node2", () => null!));
        Nodes.Add(new TreePageNode("node3", () => null!));
        Nodes.Add(new TreePageNode("node4", () => null!));
        Nodes.Add(new TreePageNode("node5", () => null!));
        Nodes.Add(new TreePageNode("node1.1", () => null!, "node1"));
        Nodes.Add(new TreePageNode("node1.2", () => null!, "node1"));
        Nodes.Add(new TreePageNode("node1.3", () => null!, "node1"));
        Nodes.Add(new TreePageNode("node1.4", () => null!, "node1"));
        Nodes.Add(new TreePageNode("node1.5", () => null!, "node1"));
        Nodes.Add(new TreePageNode("node1.1.1", () => null!, "node1.1"));
        Nodes.Add(new TreePageNode("node1.1.2", () => null!, "node1.1"));
    }
}