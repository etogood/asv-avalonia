using System;
using System.Collections.Generic;
using System.Composition;
using Asv.Common;
using Material.Icons;
using Microsoft.Extensions.Logging;
using R3;

namespace Asv.Avalonia.Example;

[ExportControlExamples(PageId)]
public class HistoricalControlsPageViewModel : ControlsGallerySubPage
{
    public const string PageId = "historical_controls";
    public const MaterialIconKind PageIcon = MaterialIconKind.History;

    private readonly ReactiveProperty<GeoPoint> _geoPointProperty;
    private readonly ReactiveProperty<bool> _isTurnedOn;
    private readonly ReactiveProperty<double> _speed;
    private readonly ReactiveProperty<string?> _stringWithManyValidations;
    private readonly ReactiveProperty<string?> _stringWithOneValidation;
    private readonly ReactiveProperty<string?> _stringWithoutValidation;

    public HistoricalControlsPageViewModel()
        : this(DesignTime.UnitService, DesignTime.LoggerFactory)
    {
        DesignTime.ThrowIfNotDesignMode();
    }

    [ImportingConstructor]
    public HistoricalControlsPageViewModel(IUnitService unit, ILoggerFactory loggerFactory)
        : base(PageId, loggerFactory)
    {
        var un = unit.Units[VelocityBase.Id];
        var latUnit = unit.Units[LatitudeBase.Id];
        var lonUnit = unit.Units[LongitudeBase.Id];
        var altUnit = unit.Units[AltitudeBase.Id];

        _speed = new ReactiveProperty<double>(double.NaN).DisposeItWith(Disposable);
        _isTurnedOn = new ReactiveProperty<bool>().DisposeItWith(Disposable);
        _stringWithoutValidation = new ReactiveProperty<string?>().DisposeItWith(Disposable);
        _stringWithOneValidation = new ReactiveProperty<string?>().DisposeItWith(Disposable);
        _stringWithManyValidations = new ReactiveProperty<string?>().DisposeItWith(Disposable);
        _geoPointProperty = new ReactiveProperty<GeoPoint>().DisposeItWith(Disposable);

        IsTurnedOn = new HistoricalBoolProperty(
            nameof(IsTurnedOn),
            _isTurnedOn,
            loggerFactory,
            this
        ).DisposeItWith(Disposable);

        TurnOn = new ReactiveCommand(_ =>
            IsTurnedOn.ViewValue.Value = !IsTurnedOn.ViewValue.Value
        ).DisposeItWith(Disposable);

        Speed = new HistoricalUnitProperty(
            nameof(Speed),
            _speed,
            un,
            loggerFactory,
            this
        ).DisposeItWith(Disposable);

        StringPropWithoutValidation = new HistoricalStringProperty(
            nameof(StringPropWithoutValidation),
            _stringWithoutValidation,
            loggerFactory,
            this
        ).DisposeItWith(Disposable);

        StringPropWithOneValidation = new HistoricalStringProperty(
            nameof(StringPropWithOneValidation),
            _stringWithOneValidation,
            loggerFactory,
            this,
            [
                v =>
                {
                    if (string.IsNullOrWhiteSpace(v))
                    {
                        return ValidationResult.FailAsNullOrWhiteSpace;
                    }

                    return ValidationResult.Success;
                },
            ]
        ).DisposeItWith(Disposable);
        StringPropWithOneValidation.ForceValidate();

        StringPropWithManyValidations = new HistoricalStringProperty(
            nameof(StringPropWithManyValidations),
            _stringWithManyValidations,
            loggerFactory,
            this,
            [
                v =>
                {
                    if (string.IsNullOrWhiteSpace(v))
                    {
                        return ValidationResult.FailAsNullOrWhiteSpace;
                    }
                    return ValidationResult.Success;
                },
                v =>
                {
                    if (v?.Contains('s', StringComparison.InvariantCultureIgnoreCase) ?? false)
                    {
                        return new ValidationResult
                        {
                            IsSuccess = false,
                            ValidationException = new ValidationException(
                                "Value shouldn't contain \'s\'"
                            ),
                        };
                    }

                    return ValidationResult.Success;
                },
            ]
        ).DisposeItWith(Disposable);
        StringPropWithManyValidations.ForceValidate();

        GeoPointProperty = new HistoricalGeoPointProperty(
            nameof(GeoPointProperty),
            _geoPointProperty,
            latUnit,
            lonUnit,
            altUnit,
            loggerFactory,
            this
        ).DisposeItWith(Disposable);
        GeoPointProperty.ForceValidate();
    }

    public ReactiveCommand TurnOn { get; }
    public HistoricalUnitProperty Speed { get; }
    public HistoricalBoolProperty IsTurnedOn { get; }
    public HistoricalStringProperty StringPropWithoutValidation { get; }
    public HistoricalStringProperty StringPropWithOneValidation { get; }
    public HistoricalStringProperty StringPropWithManyValidations { get; }
    public HistoricalGeoPointProperty GeoPointProperty { get; }

    public override IEnumerable<IRoutable> GetRoutableChildren()
    {
        yield return IsTurnedOn;
        yield return Speed;
        yield return StringPropWithoutValidation;
        yield return StringPropWithOneValidation;
        yield return StringPropWithManyValidations;
        yield return GeoPointProperty;

        foreach (var child in base.GetRoutableChildren())
        {
            yield return child;
        }
    }

    public override IExportInfo Source => SystemModule.Instance;
}
