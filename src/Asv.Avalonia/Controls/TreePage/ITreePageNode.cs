using Material.Icons;
using R3;

namespace Asv.Avalonia;

public interface ITreePageNode : IViewModel
{
    string? ParentId { get; }
    BindableReactiveProperty<string> Name { get; }
    BindableReactiveProperty<MaterialIconKind> Icon { get; }
    BindableReactiveProperty<string?> Status { get; }
    IRoutable CreateNodeViewModel();
}

public class TreePageNode : ViewModelBase, ITreePageNode
{
    private readonly Func<IRoutable> _create;

    public TreePageNode(string id, Func<IRoutable> create, string? parentId = null)
        : base(id)
    {
        ParentId = parentId;
        _create = create;
        Name = new BindableReactiveProperty<string>(id);
        Icon = new BindableReactiveProperty<MaterialIconKind>(MaterialIconKind.Tree);
        Status = new BindableReactiveProperty<string?>(null);
    }

    public string? ParentId { get; }
    public BindableReactiveProperty<string> Name { get; }
    public BindableReactiveProperty<MaterialIconKind> Icon { get; }
    public BindableReactiveProperty<string?> Status { get; }

    public IRoutable CreateNodeViewModel() => _create();

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Name.Dispose();
            Icon.Dispose();
        }
    }
}