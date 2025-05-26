using Asv.Common;
using Avalonia.Media;
using Material.Icons;
using ObservableCollections;
using R3;

namespace Asv.Avalonia.Mapping;

public interface IMapAnchor : IRoutable
{
    MaterialIconKind Icon { get; }
    string Title { get; }
    double Azimuth { get; }
    GeoPoint Location { get; }
    IBrush Foreground { get; }
    HorizontalOffset CenterX { get; }
    VerticalOffset CenterY { get; }
    bool IsReadOnly { get; }
    bool IsSelected { get; }
    bool IsVisible { get; }
    IPen? PolygonPen { get; }
    bool IsPolygonClosed { get; }
    NotifyCollectionChangedSynchronizedViewList<GeoPoint> PolygonView { get; }
}

public class MapAnchor<TContext> : ExtendableViewModel<TContext>, IMapAnchor
    where TContext : class, IMapAnchor
{
    private MaterialIconKind _icon;
    private GeoPoint _location;
    private double _azimuth;
    private IBrush _foreground = Brushes.NavajoWhite;
    private HorizontalOffset _centerX;
    private VerticalOffset _centerY;
    private bool _isReadOnly;
    private bool _isSelected;
    private bool _isVisible = true;
    private string? _title;
    private IPen? _polygonPen;
    private bool _isPolygonClosed;

    public MapAnchor(string id)
        : base(id)
    {
        Title = id;
        Polygon = new ObservableList<GeoPoint>();
        PolygonView = Polygon.ToNotifyCollectionChangedSlim().DisposeItWith(Disposable);
    }

    public double Azimuth
    {
        get => _azimuth;
        set => SetField(ref _azimuth, value);
    }

    public GeoPoint Location
    {
        get => _location;
        set => SetField(ref _location, value);
    }

    public MaterialIconKind Icon
    {
        get => _icon;
        set => SetField(ref _icon, value);
    }

    public IBrush Foreground
    {
        get => _foreground;
        set => SetField(ref _foreground, value);
    }

    public HorizontalOffset CenterX
    {
        get => _centerX;
        set => SetField(ref _centerX, value);
    }

    public VerticalOffset CenterY
    {
        get => _centerY;
        set => SetField(ref _centerY, value);
    }

    public bool IsReadOnly
    {
        get => _isReadOnly;
        set => SetField(ref _isReadOnly, value);
    }

    public bool IsSelected
    {
        get => _isSelected;
        set => SetField(ref _isSelected, value);
    }

    public bool IsVisible
    {
        get => _isVisible;
        set => SetField(ref _isVisible, value);
    }

    public IPen? PolygonPen
    {
        get => _polygonPen;
        set => SetField(ref _polygonPen, value);
    }

    public bool IsPolygonClosed
    {
        get => _isPolygonClosed;
        set => SetField(ref _isPolygonClosed, value);
    }

    public ObservableList<GeoPoint> Polygon { get; }

    public NotifyCollectionChangedSynchronizedViewList<GeoPoint> PolygonView { get; }

    public string? Title
    {
        get => _title;
        set => SetField(ref _title, value);
    }

    public override ValueTask<IRoutable> Navigate(NavigationId id)
    {
        return ValueTask.FromResult<IRoutable>(this);
    }

    public override IEnumerable<IRoutable> GetRoutableChildren()
    {
        return [];
    }

    protected override void AfterLoadExtensions()
    {
        // do nothing
    }
}
