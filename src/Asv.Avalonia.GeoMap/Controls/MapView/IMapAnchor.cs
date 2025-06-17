using Asv.Common;
using Avalonia.Media;
using Material.Icons;
using Microsoft.Extensions.Logging;
using ObservableCollections;
using R3;

namespace Asv.Avalonia.GeoMap;

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
    public MapAnchor(NavigationId id, ILoggerFactory loggerFactory)
        : base(id, loggerFactory)
    {
        Title = id.ToString();
        Polygon = new ObservableList<GeoPoint>();
        PolygonView = Polygon.ToNotifyCollectionChangedSlim().DisposeItWith(Disposable);
    }

    public double Azimuth
    {
        get;
        set => SetField(ref field, value);
    }

    public GeoPoint Location
    {
        get;
        set => SetField(ref field, value);
    }

    public MaterialIconKind Icon
    {
        get;
        set => SetField(ref field, value);
    }

    public IBrush Foreground
    {
        get;
        set => SetField(ref field, value);
    } = Brushes.NavajoWhite;

    public HorizontalOffset CenterX
    {
        get;
        set => SetField(ref field, value);
    }

    public VerticalOffset CenterY
    {
        get;
        set => SetField(ref field, value);
    }

    public bool IsReadOnly
    {
        get;
        set => SetField(ref field, value);
    }

    public bool IsSelected
    {
        get;
        set => SetField(ref field, value);
    }

    public bool IsVisible
    {
        get;
        set => SetField(ref field, value);
    } = true;

    public IPen? PolygonPen
    {
        get;
        set => SetField(ref field, value);
    }

    public bool IsPolygonClosed
    {
        get;
        set => SetField(ref field, value);
    }

    public ObservableList<GeoPoint> Polygon { get; }

    public NotifyCollectionChangedSynchronizedViewList<GeoPoint> PolygonView { get; }

    public string? Title
    {
        get;
        set => SetField(ref field, value);
    }

    public override ValueTask<IRoutable> Navigate(NavigationId id)
    {
        return ValueTask.FromResult<IRoutable>(this);
    }

    public override IEnumerable<IRoutable> GetRoutableChildren()
    {
        yield break;
    }

    protected override void AfterLoadExtensions()
    {
        // do nothing
    }
}
