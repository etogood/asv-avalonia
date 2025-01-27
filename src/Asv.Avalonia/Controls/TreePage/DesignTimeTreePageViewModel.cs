using ObservableCollections;
using R3;

namespace Asv.Avalonia;

public interface IDesignTimeTreePage : IPage
{
    BindableReactiveProperty<bool> IsCompactMode { get; }
    IEnumerable<TreeMenuItem>? Items { get; }
    BindableReactiveProperty<TreeMenuItem?> SelectedMenu { get; }
    BindableReactiveProperty<IRoutable?> SelectedPage { get; }
    ISynchronizedViewList<BreadCrumbItem> BreadCrumb { get; }
}

public class DesignTimeTreePageViewModel : TreePageViewModel<IPage>
{
    public DesignTimeTreePageViewModel()
        : base(DesignTime.Id, DesignTime.CommandService)
    {
        DesignTime.ThrowIfNotDesignMode();
        Nodes.Add(new TreePageNode("node1", () => new SettingsAppearanceViewModel()));
        Nodes.Add(new TreePageNode("node2", () => new SettingsAppearanceViewModel()));
        Nodes.Add(new TreePageNode("node3", () => new SettingsAppearanceViewModel()));
        Nodes.Add(new TreePageNode("node4", () => new SettingsAppearanceViewModel()));
        Nodes.Add(new TreePageNode("node5", () => new SettingsAppearanceViewModel()));
        Nodes.Add(new TreePageNode("node1.1", () => new SettingsAppearanceViewModel(), "node1"));
        Nodes.Add(new TreePageNode("node1.2", () => new SettingsAppearanceViewModel(), "node1"));
        Nodes.Add(new TreePageNode("node1.3", () => new SettingsAppearanceViewModel(), "node1"));
        Nodes.Add(new TreePageNode("node1.4", () => new SettingsAppearanceViewModel(), "node1"));
        Nodes.Add(new TreePageNode("node1.5", () => new SettingsAppearanceViewModel(), "node1"));
        Nodes.Add(new TreePageNode("node1.1.1", () => new SettingsAppearanceViewModel(), "node1.1"));
        Nodes.Add(new TreePageNode("node1.1.2", () => new SettingsAppearanceViewModel(), "node1.1"));

        Init();
    }
}