using System;
using System.Collections.Generic;
using System.Composition;
using Material.Icons;
using Microsoft.Extensions.Logging;
using R3;

namespace Asv.Avalonia.Example;

[ExportPage(PageId)]
public class TestHistoryPropertiesPageViewModel : PageViewModel<TestHistoryPropertiesPageViewModel>
{
    public const string PageId = "test.history.properties";
    public const MaterialIconKind PageIcon = MaterialIconKind.TestTube;

    private readonly ReactiveProperty<double> _speed;
    private readonly ReactiveProperty<bool> _isTurnedOn;
    private readonly ReactiveProperty<string?> _stringWithoutValidation;
    private readonly ReactiveProperty<string?> _stringWithOneValidation;
    private readonly ReactiveProperty<string?> _stringWithManyValidations;

    public TestHistoryPropertiesPageViewModel()
        : this(DesignTime.UnitService, DesignTime.CommandService, DesignTime.LoggerFactory)
    {
        DesignTime.ThrowIfNotDesignMode();
    }

    [ImportingConstructor]
    public TestHistoryPropertiesPageViewModel(
        IUnitService unit,
        ICommandService commandService,
        ILoggerFactory loggerFactory
    )
        : base(PageId, commandService, loggerFactory)
    {
        Title = "Test History Properties";
        var un = unit.Units[VelocityBase.Id];
        _speed = new ReactiveProperty<double>(double.NaN);
        _isTurnedOn = new ReactiveProperty<bool>();
        _stringWithoutValidation = new ReactiveProperty<string?>();
        _stringWithOneValidation = new ReactiveProperty<string?>();
        _stringWithManyValidations = new ReactiveProperty<string?>();

        IsTurnedOn = new HistoricalBoolProperty(
            $"{PageId}.{nameof(Speed)}",
            _isTurnedOn,
            loggerFactory
        )
        {
            Parent = this,
        };

        TurnOn = new ReactiveCommand(_ => IsTurnedOn.ViewValue.Value = !IsTurnedOn.ViewValue.Value);

        Speed = new HistoricalUnitProperty($"{PageId}.{nameof(Speed)}", _speed, un, loggerFactory)
        {
            Parent = this,
        };
        StringPropWithoutValidation = new HistoricalStringProperty(
            $"{PageId}.{nameof(StringPropWithoutValidation)}",
            _stringWithoutValidation,
            loggerFactory
        )
        {
            Parent = this,
        };

        StringPropWithOneValidation = new HistoricalStringProperty(
            $"{PageId}.{nameof(StringPropWithOneValidation)}",
            _stringWithOneValidation,
            loggerFactory,
            [
                v =>
                {
                    if (string.IsNullOrWhiteSpace(v))
                    {
                        return new Exception("Value shouldn't be empty");
                    }

                    return ValidationResult.Success;
                },
            ]
        )
        {
            Parent = this,
        };

        StringPropWithManyValidations = new HistoricalStringProperty(
            $"{PageId}.{nameof(StringPropWithManyValidations)}",
            _stringWithManyValidations,
            loggerFactory,
            [
                v =>
                {
                    if (string.IsNullOrWhiteSpace(v))
                    {
                        return new Exception("Value shouldn't be empty");
                    }

                    return ValidationResult.Success;
                },
                v =>
                {
                    if (v?.Contains('s', StringComparison.InvariantCultureIgnoreCase) ?? false)
                    {
                        return new Exception("Value shouldn't contain \'s\'");
                    }

                    return ValidationResult.Success;
                },
            ]
        )
        {
            Parent = this,
        };
    }

    public ReactiveCommand TurnOn { get; }
    public HistoricalUnitProperty Speed { get; }
    public HistoricalBoolProperty IsTurnedOn { get; }
    public HistoricalStringProperty StringPropWithoutValidation { get; }
    public HistoricalStringProperty StringPropWithOneValidation { get; }
    public HistoricalStringProperty StringPropWithManyValidations { get; }

    protected override TestHistoryPropertiesPageViewModel GetContext()
    {
        return this;
    }

    protected override void AfterLoadExtensions() { }

    public override IEnumerable<IRoutable> GetRoutableChildren()
    {
        yield return IsTurnedOn;
        yield return Speed;
        yield return StringPropWithoutValidation;
        yield return StringPropWithOneValidation;
    }

    public override IExportInfo Source => SystemModule.Instance;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            TurnOn.Dispose();
            _speed.Dispose();
            _isTurnedOn.Dispose();
            _stringWithoutValidation.Dispose();
            _stringWithOneValidation.Dispose();
            _stringWithManyValidations.Dispose();
            Speed.Dispose();
            StringPropWithoutValidation.Dispose();
            StringPropWithOneValidation.Dispose();
            StringPropWithManyValidations.Dispose();
        }

        base.Dispose(disposing);
    }
}
