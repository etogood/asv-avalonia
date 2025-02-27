using Asv.Common;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Mixins;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;

namespace Asv.Avalonia.Map;

[PseudoClasses(":pressed", ":selected", ":pointerover")]
public class MapViewItem : ContentControl, ISelectable
{
    static MapViewItem()
    {
        SelectableMixin.Attach<MapViewItem>(IsSelectedProperty);
        PressedMixin.Attach<MapViewItem>();
    }

    public MapViewItem() { }

    protected override void OnPointerEntered(PointerEventArgs e)
    {
        PseudoClasses.Add(":pointerover");
    }

    protected override void OnPointerExited(PointerEventArgs e)
    {
        PseudoClasses.Remove(":pointerover");
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            PseudoClasses.Add(":pressed");
        }
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        PseudoClasses.Remove(":pressed");
    }

    #region IsSelected

    public static readonly StyledProperty<bool> IsSelectedProperty = AvaloniaProperty.Register<
        MapViewItem,
        bool
    >(nameof(IsSelected));

    public bool IsSelected
    {
        get => GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }

    #endregion
}
