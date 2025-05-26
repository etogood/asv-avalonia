using Asv.Common;
using R3;

namespace Asv.Avalonia;

public sealed class HistoricalGeoPointProperty : CompositeHistoricalPropertyBase<GeoPoint>
{
    private readonly ReactiveProperty<GeoPoint> _modelValue;

    public HistoricalGeoPointProperty(
        string id,
        ReactiveProperty<GeoPoint> modelValue,
        IUnit latUnit,
        IUnit lonUnit,
        IUnit altUnit
    )
        : base(id)
    {
        _modelValue = modelValue;
        var modelLat = new ReactiveProperty<double>().DisposeItWith(Disposable);
        modelLat
            .Subscribe(x =>
            {
                _modelValue.Value = new GeoPoint(
                    x,
                    _modelValue.Value.Longitude,
                    _modelValue.Value.Altitude
                );
            })
            .DisposeItWith(Disposable);

        var modelLon = new ReactiveProperty<double>().DisposeItWith(Disposable);
        modelLon
            .Subscribe(x =>
            {
                _modelValue.Value = new GeoPoint(
                    _modelValue.Value.Latitude,
                    x,
                    _modelValue.Value.Altitude
                );
            })
            .DisposeItWith(Disposable);

        var modelAlt = new ReactiveProperty<double>().DisposeItWith(Disposable);
        modelAlt
            .Subscribe(x =>
            {
                _modelValue.Value = new GeoPoint(
                    _modelValue.Value.Latitude,
                    _modelValue.Value.Longitude,
                    x
                );
            })
            .DisposeItWith(Disposable);

        Latitude = new HistoricalUnitProperty("lat", modelLat, latUnit)
        {
            Parent = this,
        }.DisposeItWith(Disposable);
        Longitude = new HistoricalUnitProperty("lon", modelLon, lonUnit)
        {
            Parent = this,
        }.DisposeItWith(Disposable);
        Altitude = new HistoricalUnitProperty("alt", modelAlt, altUnit)
        {
            Parent = this,
        }.DisposeItWith(Disposable);

        _modelValue
            .Subscribe(x =>
            {
                modelLat.Value = x.Latitude;
                modelLon.Value = x.Longitude;
                modelAlt.Value = x.Altitude;
            })
            .DisposeItWith(Disposable);
    }

    public override IEnumerable<IRoutable> GetRoutableChildren()
    {
        yield return Latitude;
        yield return Longitude;
        yield return Altitude;
    }

    public HistoricalUnitProperty Latitude { get; }
    public HistoricalUnitProperty Longitude { get; }
    public HistoricalUnitProperty Altitude { get; }

    public override ReactiveProperty<GeoPoint> ModelValue => _modelValue;
}
