using System;
using System.Collections.Generic;
using System.Composition;
using R3;

namespace Asv.Avalonia.Example;

[ExportPage(PageId)]
public class TestUnitsPageViewModel : PageViewModel<TestUnitsPageViewModel>
{
    public const string PageId = "test.history.properties";

    private ReactiveProperty<double> _speed;
    private ReactiveProperty<string?> _stringWithoutValidation;
    private ReactiveProperty<string?> _stringWithOneValidation;
    private ReactiveProperty<string?> _stringWithManyValidations;

    public TestUnitsPageViewModel()
        : this(DesignTime.UnitService, DesignTime.CommandService)
    {
        DesignTime.ThrowIfNotDesignMode();
    }

    [ImportingConstructor]
    public TestUnitsPageViewModel(IUnitService unit, ICommandService commandService)
        : base(PageId, commandService)
    {
        Title.OnNext("Test History Properties");
        var un = unit.Units[VelocityBase.Id];
        _speed = new ReactiveProperty<double>(double.NaN);
        _stringWithoutValidation = new ReactiveProperty<string?>();
        _stringWithOneValidation = new ReactiveProperty<string?>();
        _stringWithManyValidations = new ReactiveProperty<string?>();

        Speed = new HistoricalUnitProperty($"{PageId}.{nameof(Speed)}", _speed, un)
        {
            Parent = this,
        };
        StringPropWithoutValidation = new HistoricalStringProperty(
            $"{PageId}.{nameof(StringPropWithoutValidation)}",
            _stringWithoutValidation
        )
        {
            Parent = this,
        };

        StringPropWithOneValidation = new HistoricalStringProperty(
            $"{PageId}.{nameof(StringPropWithOneValidation)}",
            _stringWithOneValidation,
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

    public HistoricalUnitProperty Speed { get; }
    public HistoricalStringProperty StringPropWithoutValidation { get; }
    public HistoricalStringProperty StringPropWithOneValidation { get; }
    public HistoricalStringProperty StringPropWithManyValidations { get; }

    protected override TestUnitsPageViewModel GetContext()
    {
        return this;
    }

    protected override void AfterLoadExtensions() { }

    public override IEnumerable<IRoutable> GetRoutableChildren()
    {
        yield return Speed;
        yield return StringPropWithoutValidation;
        yield return StringPropWithOneValidation;
    }

    public override IExportInfo Source => SystemModule.Instance;
}
