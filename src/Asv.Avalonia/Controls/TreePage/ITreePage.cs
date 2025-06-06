using Asv.Avalonia.Routable;
using Material.Icons;
using R3;

namespace Asv.Avalonia;

public interface ITreePage : IHeadlinedViewModel
{
    Routable.NavigationId ParentId { get; }
    string? Status { get; }
    Routable.NavigationId NavigateTo { get; }
    ReactiveCommand NavigateCommand { get; }
}

public class TreePage : HeadlinedViewModel, ITreePage
{
    private string? _status;

    public TreePage(
        Routable.NavigationId id,
        string title,
        MaterialIconKind? icon,
        Routable.NavigationId navigateTo,
        Routable.NavigationId parentId
    )
        : base(id)
    {
        NavigateTo = navigateTo;
        ParentId = parentId;
        Header = title;
        Icon = icon;
    }

    public Routable.NavigationId NavigateTo { get; }
    public ReactiveCommand NavigateCommand { get; }
    public Routable.NavigationId ParentId { get; }

    public string? Status
    {
        get => _status;
        set => SetField(ref _status, value);
    }

    public override IEnumerable<IRoutable> GetRoutableChildren()
    {
        return [];
    }
}
