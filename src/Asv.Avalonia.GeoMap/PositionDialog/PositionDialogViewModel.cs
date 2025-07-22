using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Composition;
using System.Windows.Input;
using Asv.Avalonia;
using Asv.Common;
using Microsoft.Extensions.Logging;
using R3;

namespace Asv.Avalonia.GeoMap;

public class PositionDialogViewModel : DialogViewModelBase
{
    public const string ViewModelId = "Asv.Drones.PositioningDialog";

    [ImportingConstructor]
    public PositionDialogViewModel(NavigationId id, ILoggerFactory loggerFactory)
        : base(id, loggerFactory)
    {
        XCoord = new BindableReactiveProperty<double>(0).DisposeItWith(Disposable);
        YCoord = new BindableReactiveProperty<double>(0).DisposeItWith(Disposable);
        ZCoord = new BindableReactiveProperty<double>(0).DisposeItWith(Disposable);
        SelectedStep = new BindableReactiveProperty<double>(1.0).DisposeItWith(Disposable);

        StepOptions = new ObservableCollection<double> { 0.1, 0.5, 1.0, 2.0, 5.0 };

        MoveUpCommand = new ReactiveCommand(_ => YCoord.Value += SelectedStep.Value).DisposeItWith(
            Disposable
        );
        MoveDownCommand = new ReactiveCommand(_ =>
            YCoord.Value -= SelectedStep.Value
        ).DisposeItWith(Disposable);
        MoveLeftCommand = new ReactiveCommand(_ =>
            XCoord.Value -= SelectedStep.Value
        ).DisposeItWith(Disposable);
        MoveRightCommand = new ReactiveCommand(_ =>
            XCoord.Value += SelectedStep.Value
        ).DisposeItWith(Disposable);

        MoveTopLeftCommand = new ReactiveCommand(_ =>
        {
            XCoord.Value -= SelectedStep.Value;
            YCoord.Value += SelectedStep.Value;
        }).DisposeItWith(Disposable);

        MoveTopRightCommand = new ReactiveCommand(_ =>
        {
            XCoord.Value += SelectedStep.Value;
            YCoord.Value += SelectedStep.Value;
        }).DisposeItWith(Disposable);

        MoveBottomLeftCommand = new ReactiveCommand(_ =>
        {
            XCoord.Value -= SelectedStep.Value;
            YCoord.Value -= SelectedStep.Value;
        }).DisposeItWith(Disposable);

        MoveBottomRightCommand = new ReactiveCommand(_ =>
        {
            XCoord.Value += SelectedStep.Value;
            YCoord.Value -= SelectedStep.Value;
        }).DisposeItWith(Disposable);

        IncreaseZCommand = new ReactiveCommand(_ =>
            ZCoord.Value += SelectedStep.Value
        ).DisposeItWith(Disposable);
        DecreaseZCommand = new ReactiveCommand(_ =>
            ZCoord.Value -= SelectedStep.Value
        ).DisposeItWith(Disposable);

        ConfirmCommand = new ReactiveCommand(_ =>
        { /* Подтверждение */
        }).DisposeItWith(Disposable);
    }

    public BindableReactiveProperty<double> XCoord { get; }
    public BindableReactiveProperty<double> YCoord { get; }
    public BindableReactiveProperty<double> ZCoord { get; }
    public BindableReactiveProperty<double> SelectedStep { get; }

    public ObservableCollection<double> StepOptions { get; }

    public ICommand ConfirmCommand { get; }
    public ICommand MoveUpCommand { get; }
    public ICommand MoveDownCommand { get; }
    public ICommand MoveLeftCommand { get; }
    public ICommand MoveRightCommand { get; }
    public ICommand MoveTopLeftCommand { get; }
    public ICommand MoveTopRightCommand { get; }
    public ICommand MoveBottomLeftCommand { get; }
    public ICommand MoveBottomRightCommand { get; }
    public ICommand IncreaseZCommand { get; }
    public ICommand DecreaseZCommand { get; }

    public override IEnumerable<IRoutable> GetRoutableChildren()
    {
        yield break;
    }
}
