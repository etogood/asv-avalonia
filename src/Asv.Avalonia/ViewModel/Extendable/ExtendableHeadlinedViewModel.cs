using Avalonia.Media;
using Material.Icons;

namespace Asv.Avalonia;

public abstract class ExtendableHeadlinedViewModel<TSelfInterface>(NavigationId id)
    : ExtendableViewModel<TSelfInterface>(id),
        IHeadlinedViewModel
    where TSelfInterface : class
{
    private MaterialIconKind? _icon;
    private IBrush? _iconBrush = Brushes.BlueViolet;
    private string? _header;
    private string? _description;
    private int _order;
    private bool _isVisible = true;

    public MaterialIconKind? Icon
    {
        get => _icon;
        set => SetField(ref _icon, value);
    }

    public IBrush? IconBrush
    {
        get => _iconBrush;
        set => SetField(ref _iconBrush, value);
    }

    public string? Header
    {
        get => _header;
        set => SetField(ref _header, value);
    }

    public string? Description
    {
        get => _description;
        set => SetField(ref _description, value);
    }

    public bool IsVisible
    {
        get => _isVisible;
        set => SetField(ref _isVisible, value);
    }

    public int Order
    {
        get => _order;
        set => SetField(ref _order, value);
    }
}
