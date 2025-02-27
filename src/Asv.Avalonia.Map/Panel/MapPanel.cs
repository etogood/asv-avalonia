using Asv.Common;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using R3;

namespace Asv.Avalonia.Map;

public class MapPanel : Panel
{
    static MapPanel()
    {
        AffectsParentArrange<MapPanel>(LocationProperty);
        AffectsArrange<MapPanel>(ProviderProperty, ZoomProperty, CenterMapProperty);
    }

    public MapPanel()
    {
        Provider = new BingTileProvider(
            "Anqg-XzYo-sBPlzOWFHIcjC3F8s17P_O7L4RrevsHVg4fJk6g_eEmUBphtSn4ySg"
        );
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        availableSize = new Size(double.PositiveInfinity, double.PositiveInfinity);

        foreach (Control child in Children)
        {
            child.Measure(availableSize);
        }

        return new Size();
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        foreach (Control child in Children)
        {
            ArrangeChild(child, finalSize);
        }

        return finalSize;
    }

    protected virtual void ArrangeChild(Control child, Size finalSize)
    {
        var point = GetLocation(child);
        var offsetH = GetHorizontalOffset(child);
        var offsetV = GetVerticalOffset(child);

        if (Provider == null || point == null)
            return;

        var tileSize = Provider.TileSize;
        var halfWidth = Bounds.Width * 0.5;
        var halfHeight = Bounds.Height * 0.5;
        var projection = Provider.Projection;

        var centerPixel = projection.Wgs84ToPixels(CenterMap, Zoom, tileSize);
        var offset = new Point(halfWidth - centerPixel.X, halfHeight - centerPixel.Y);
        var pos = projection.Wgs84ToPixels(point.Value, Zoom, tileSize);

        pos += offset;
        pos = new Point(
            pos.X + offsetH.CalculateOffset(child.Bounds.Width),
            pos.Y + offsetV.CalculateOffset(child.Bounds.Height)
        );

        child.Arrange(new Rect(pos, child.DesiredSize));
    }

    #region VerticalOffset

    public static readonly AttachedProperty<VerticalOffset> VerticalOffsetProperty =
        AvaloniaProperty.RegisterAttached<MapPanel, Control, VerticalOffset>("VerticalOffset");

    public static void SetVerticalOffset(Control obj, VerticalOffset value) =>
        obj.SetValue(VerticalOffsetProperty, value);

    public static VerticalOffset GetVerticalOffset(Control obj) =>
        obj.GetValue(VerticalOffsetProperty);

    #endregion

    #region HorizontalOffset

    public static readonly AttachedProperty<HorizontalOffset> HorizontalOffsetProperty =
        AvaloniaProperty.RegisterAttached<MapPanel, Control, HorizontalOffset>("HorizontalOffset");

    public static void SetHorizontalOffset(Control obj, HorizontalOffset value) =>
        obj.SetValue(HorizontalOffsetProperty, value);

    public static HorizontalOffset GetHorizontalOffset(Control obj) =>
        obj.GetValue(HorizontalOffsetProperty);

    #endregion

    #region Location

    public static readonly AttachedProperty<GeoPoint?> LocationProperty =
        AvaloniaProperty.RegisterAttached<MapPanel, Control, GeoPoint?>("Location");

    public static GeoPoint? GetLocation(Control element) => element.GetValue(LocationProperty);

    public static void SetLocation(Control element, GeoPoint? value) =>
        element.SetValue(LocationProperty, value);

    #endregion

    #region CenterMap

    public static readonly StyledProperty<GeoPoint> CenterMapProperty = AvaloniaProperty.Register<
        MapPanel,
        GeoPoint
    >(nameof(CenterMap));

    public GeoPoint CenterMap
    {
        get => GetValue(CenterMapProperty);
        set => SetValue(CenterMapProperty, value);
    }

    #endregion

    #region Provider

    public static readonly StyledProperty<ITileProvider?> ProviderProperty =
        AvaloniaProperty.Register<MapPanel, ITileProvider?>(
            nameof(Provider),
            coerce: (o, provider) => provider ?? EmptyTileProvider.Instance
        );

    public ITileProvider? Provider
    {
        get => GetValue(ProviderProperty);
        set => SetValue(ProviderProperty, value);
    }

    #endregion

    #region Zoom

    public static readonly StyledProperty<int> ZoomProperty = AvaloniaProperty.Register<
        MapPanel,
        int
    >(nameof(Zoom));

    public int Zoom
    {
        get => GetValue(ZoomProperty);
        set => SetValue(ZoomProperty, value);
    }

    #endregion
}
