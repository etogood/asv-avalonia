using System.Diagnostics;
using Asv.Common;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Mixins;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.VisualTree;

namespace Asv.Avalonia.Map;

public enum DragState
{
    None,
    DragSelection,
    DragMap,
}

public class MapView : SelectingItemsControl
{
    private Point _lastMousePosition;
    private DragState _dragState = DragState.None;

    public MapView()
    {
        Provider = new BingTileProvider(
            "Anqg-XzYo-sBPlzOWFHIcjC3F8s17P_O7L4RrevsHVg4fJk6g_eEmUBphtSn4ySg"
        );
        SelectionMode = SelectionMode.Multiple;
        SelectedItems = new AvaloniaList<object>();
    }

    protected override Control CreateContainerForItemOverride(
        object? item,
        int index,
        object? recycleKey
    )
    {
        return new MapViewItem();
    }

    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
    {
        return NeedsContainer<MapViewItem>(item, out recycleKey);
    }

    protected override void PrepareContainerForItemOverride(
        Control container,
        object item,
        int index
    )
    {
        base.PrepareContainerForItemOverride(container, item, index);
        if (container is MapViewItem listBoxItem)
        {
            listBoxItem.IsSelected = SelectedItems.Contains(item);
        }
    }

    #region Pointer Events



    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        _lastMousePosition = e.GetPosition(this);

        if (UpdateSelectionFromEventSource(e.Source, toggleModifier: true))
        {
            /*var item = ItemFromContainer(container);
            if (item != null)
            {
                
                if (e.KeyModifiers == KeyModifiers.Control)
                {
                    // Ctrl + клик: добавить/убрать элемент
                    if (SelectedItems.Contains(item))
                    {
                        SelectedItems.Remove(item);
                        container.IsSelected = false;
                    }
                    else
                    {
                        SelectedItems.Add(item);
                        container.IsSelected = true;
                    }
                        
                }
                else if (e.KeyModifiers == KeyModifiers.Shift && SelectedItem != null)
                {
                    if (SelectedItems.Contains(item) == false)
                    {
                        SelectedItems.Add(item);
                        container.IsSelected = true;
                    }
                }
                else
                {
                    // Обычный клик: сбросить выбор и выбрать один элемент
                    foreach (var selectedItem in SelectedItems)
                    {
                        var ctrl = ContainerFromItem(selectedItem);
                        if (ctrl != null)
                        {
                            if (ctrl is MapViewItem c)
                            {
                                c.IsSelected = false;
                            }
                        }
                    }
                    SelectedItems.Clear();
                    SelectedItems.Add(item);
                    container.IsSelected = true;
                }
                Debug.WriteLine(SelectedItems.Count);
                SelectedItem = item;
                Debug.WriteLine(SelectedItems.Count);
            }*/
            _dragState = DragState.DragSelection;
        }
        else
        {
            _dragState = DragState.DragMap;
        }
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);

        var offset =
            new Point(Bounds.Width / 2, Bounds.Height / 2)
            - Provider.Projection.Wgs84ToPixels(CenterMap, Zoom, Provider.TileSize);

        var currentPosition = e.GetPosition(this);

        CursorPosition = Provider.Projection.PixelsToWgs84(
            currentPosition - offset,
            Zoom,
            Provider.TileSize
        );

        if (_dragState == DragState.None) { }
        else if (_dragState == DragState.DragSelection)
        {
            foreach (var item in SelectedItems)
            {
                var ctrl = ContainerFromItem(item);
                if (ctrl != null)
                {
                    var location = MapPanel.GetLocation(ctrl);
                    if (location != null)
                    {
                        var delta = currentPosition - _lastMousePosition;
                        var currentPixel = Provider.Projection.Wgs84ToPixels(
                            location.Value,
                            Zoom,
                            Provider.TileSize
                        );
                        var newPixelX = currentPixel.X + delta.X;
                        var newPixelY = currentPixel.Y + delta.Y;
                        var newLocation = Provider.Projection.PixelsToWgs84(
                            new Point(newPixelX, newPixelY),
                            Zoom,
                            Provider.TileSize
                        );
                        MapPanel.SetLocation(ctrl, newLocation);
                    }
                }
            }
        }
        else if (_dragState == DragState.DragMap)
        {
            offset = offset + currentPosition - _lastMousePosition;

            CenterMap = Provider.Projection.PixelsToWgs84(
                new Point(Bounds.Width / 2 - offset.X, Bounds.Height / 2 - offset.Y),
                Zoom,
                Provider.TileSize
            );
        }
        _lastMousePosition = currentPosition;
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        _dragState = DragState.None;
    }

    protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
    {
        base.OnPointerWheelChanged(e);
        var newZoom = _zoom;

        if (e.Delta.Y > 0 && _zoom < 19)
        {
            newZoom++;
        }
        else if (e.Delta.Y < 0 && _zoom > 1)
        {
            newZoom--;
        }
        if (newZoom != _zoom)
        {
            Zoom = newZoom;
        }
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);
    }

    #endregion

    #region Cursor position

    private GeoPoint _cursorPosition;

    public static readonly DirectProperty<MapView, GeoPoint> CursorPositionProperty =
        AvaloniaProperty.RegisterDirect<MapView, GeoPoint>(
            nameof(CursorPosition),
            o => o.CursorPosition,
            (o, v) => o.CursorPosition = v
        );

    public GeoPoint CursorPosition
    {
        get => _cursorPosition;
        set => SetAndRaise(CursorPositionProperty, ref _cursorPosition, value);
    }

    #endregion

    #region CenterMap

    private GeoPoint _centerMap;

    public static readonly DirectProperty<MapView, GeoPoint> CenterMapProperty =
        AvaloniaProperty.RegisterDirect<MapView, GeoPoint>(
            nameof(CenterMap),
            o => o.CenterMap,
            (o, v) => o.CenterMap = v
        );

    public GeoPoint CenterMap
    {
        get => _centerMap;
        set => SetAndRaise(CenterMapProperty, ref _centerMap, value);
    }

    #endregion

    #region Provider

    private ITileProvider _provider = EmptyTileProvider.Instance;
    public static readonly DirectProperty<MapView, ITileProvider> ProviderProperty =
        AvaloniaProperty.RegisterDirect<MapView, ITileProvider>(
            nameof(Provider),
            o => o.Provider,
            (o, v) => o.Provider = v
        );

    public ITileProvider Provider
    {
        get => _provider;
        set => SetAndRaise(ProviderProperty, ref _provider, value);
    }

    #endregion

    #region Zoom

    private int _zoom = 8;

    public static readonly DirectProperty<MapView, int> ZoomProperty =
        AvaloniaProperty.RegisterDirect<MapView, int>(
            nameof(Zoom),
            o => o.Zoom,
            (o, v) => o.Zoom = v
        );

    public int Zoom
    {
        get => _zoom;
        set => SetAndRaise(ZoomProperty, ref _zoom, value);
    }

    #endregion
}
