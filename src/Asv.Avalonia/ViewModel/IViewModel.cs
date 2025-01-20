using R3;

namespace Asv.Avalonia;

public interface IViewModel : IDisposable
{
    string Id { get; }
}

public interface IViewModelWithNavigation : IViewModel
{
    BindableReactiveProperty<bool> IsSelected { get; }
    IViewModelWithNavigation? Parent { get; }
    IEnumerable<IViewModelWithNavigation> Children { get; }
}

abstract class ViewModelWithNavigation : ViewModelBase, IViewModelWithNavigation
{
    private readonly IViewModelWithNavigation? _parent;

    protected ViewModelWithNavigation(string id, INavigationService nav, IViewModelWithNavigation? parent = null)
        : base(id)
    {
        _parent = parent;
        
    }

    public BindableReactiveProperty<bool> IsSelected { get; } = new(false);
    public IViewModelWithNavigation? Parent => _parent;
    public abstract IEnumerable<IViewModelWithNavigation> Children { get; }
}