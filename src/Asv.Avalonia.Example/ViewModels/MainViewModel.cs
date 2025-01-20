using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using R3;

namespace Asv.Avalonia.Example.ViewModels;

public class Model : IDisposable
{
    public ReactiveProperty<double> Value1 { get; } = new(0);
    public ReactiveProperty<double> Value2 { get; } = new(0);

    public void Dispose()
    {
        Value1.Dispose();
        Value2.Dispose();
    }
}


public class MainViewModel : DisposableViewModel
{
    private readonly INavigationService _nav;
    private readonly Model _model;

    public MainViewModel(IUnitService unit, ICommandService command, INavigationService nav)
        : base("shell")
    {
        _nav = nav;
        _model = new Model();
        History = command.CreateHistory(Id);
        
        var distanceUnit = unit[WellKnownUnits.Distance] ?? throw new InvalidOperationException($"{WellKnownUnits.Distance} unit not found");

        Property1 = new HistoricalUnitProperty(nameof(_model.Value1), _model.Value1, distanceUnit, History);
        Property2 = new HistoricalUnitProperty(nameof(_model.Value2), _model.Value2, distanceUnit, History);
    }

    public IEnumerable<IViewModel> GetChildren()
    {
        yield return Property1;
        yield return Property2;
    }

    public IViewModel? Parent => null;

    public ICommandHistory History { get; }
    public HistoricalUnitProperty Property2 { get; }
    public HistoricalUnitProperty Property1 { get; }
}