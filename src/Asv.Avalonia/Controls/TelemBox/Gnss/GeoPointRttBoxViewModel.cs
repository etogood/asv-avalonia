using Asv.Common;
using Material.Icons;
using Microsoft.Extensions.Logging;
using R3;

namespace Asv.Avalonia;

public class GeoPointRttBoxViewModel : RttBoxViewModel
{
    private readonly TimeSpan? _networkErrorTimeout;
    private readonly IUnitItem _latitudeUnit;
    private readonly IUnitItem _longitudeUnit;
    private readonly IUnitItem _altitudeUnit;

    public GeoPointRttBoxViewModel()
    {
        DesignTime.ThrowIfNotDesignMode();
        var start = new GeoPoint(55.75, 37.6173, 250.0); // Moscow coordinates
        var sub = new Subject<GeoPoint>();
        Observable<GeoPoint> value = sub;
        var index = 0;
        var maxIndex = Enum.GetValues<RttBoxStatus>().Length;
        Observable
            .Timer(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2))
            .Subscribe(x =>
            {
                if (Random.Shared.NextDouble() > 0.9)
                {
                    IsNetworkError = true;
                    return;
                }

                var point = new GeoPoint(
                    start.Latitude + Random.Shared.NextDouble(),
                    start.Longitude + Random.Shared.NextDouble(),
                    start.Altitude + Random.Shared.NextDouble() + 0.5
                );
                Status = Enum.GetValues<RttBoxStatus>()[index++ % maxIndex];
                ProgressStatus = Enum.GetValues<RttBoxStatus>()[index++ % maxIndex];
                Progress = Random.Shared.NextDouble();
                StatusText = Status.ToString();
                sub.OnNext(point);
                Updated();
            });
        _latitudeUnit = new DmsLatitudeUnit();
        _longitudeUnit = new DmsLongitudeUnit();
        _altitudeUnit = new MeterAltitudeUnit();
        Header = "UAV position";
        ShortHeader = "UAV";
        Icon = MaterialIconKind.AddressMarker;
        value
            .ThrottleLastFrame(1)
            .ObserveOnUIThreadDispatcher()
            .Subscribe(OnValueChanged)
            .DisposeItWith(Disposable);
    }

    public GeoPointRttBoxViewModel(
        NavigationId id,
        ILoggerFactory loggerFactory,
        IUnitService units,
        Observable<GeoPoint> value,
        TimeSpan? networkErrorTimeout
    )
        : base(id, loggerFactory, networkErrorTimeout)
    {
        _networkErrorTimeout = networkErrorTimeout;
        _latitudeUnit =
            units[LatitudeBase.Id]?.CurrentUnitItem.CurrentValue
            ?? throw new ArgumentException("Latitude unit not found in unit service");
        _longitudeUnit =
            units[LongitudeBase.Id]?.CurrentUnitItem.CurrentValue
            ?? throw new ArgumentException("Longitude unit not found in unit service");
        _altitudeUnit =
            units[AltitudeBase.Id]?.CurrentUnitItem.CurrentValue
            ?? throw new ArgumentException("Altitude unit not found in unit service");
        value
            .ThrottleLastFrame(1)
            .ObserveOnUIThreadDispatcher()
            .Subscribe(OnValueChanged)
            .DisposeItWith(Disposable);
    }

    private void OnValueChanged(GeoPoint geoPoint)
    {
        if (
            double.IsNaN(geoPoint.Latitude)
            || double.IsNaN(geoPoint.Longitude)
            || double.IsNaN(geoPoint.Altitude)
        )
        {
            LatitudeString = NotAvailableString;
            LongitudeString = NotAvailableString;
            AltitudeString = NotAvailableString;
        }
        else
        {
            LatitudeString = _latitudeUnit.PrintFromSi(geoPoint.Latitude);
            LongitudeString = _longitudeUnit.PrintFromSi(geoPoint.Longitude);
            AltitudeString = _altitudeUnit.PrintWithUnits(
                _altitudeUnit.FromSi(geoPoint.Altitude),
                "F2"
            );
        }

        if (_networkErrorTimeout != null)
        {
            Updated();
        }
    }

    public string NotAvailableString { get; set; } = "N\\A";

    public string? AltitudeString
    {
        get;
        set => SetField(ref field, value);
    }

    public string? LongitudeString
    {
        get;
        set => SetField(ref field, value);
    }

    public string? LatitudeString
    {
        get;
        set => SetField(ref field, value);
    }

    public string? StatusText
    {
        get;
        set => SetField(ref field, value);
    }

    public string? ShortStatusText
    {
        get;
        set => SetField(ref field, value);
    }
}
