using System;
using System.Collections.Generic;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Asv.Avalonia.GeoMap;
using Asv.Avalonia.Plugins;
using Material.Icons;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using R3;

namespace Asv.Avalonia.Example;

// TODO:Localize
[ExportPage(PageId)]
public class DialogBoardViewModel : PageViewModel<DialogBoardViewModel>
{
    public const string PageId = "dialog";
    public const MaterialIconKind PageIcon = MaterialIconKind.Dialogue;

    private readonly ILoggerFactory _loggerFactory;
    private readonly INavigationService _navigationService;

    private readonly SelectFolderDialogDesktopPrefab _selectFolderDialog;
    private readonly ObserveFolderDialogPrefab _observeFolderDialog;
    private readonly SaveFileDialogDesktopPrefab _saveFileDialog;
    private readonly OpenFileDialogDesktopPrefab _openFileDialog;
    private readonly SaveCancelDialogPrefab _saveCancelDialog;
    private readonly YesOrNoDialogPrefab _yesNoDialog;
    private readonly InputDialogPrefab _inputDialog; // private readonly PositionDialogPrefab DialogService;

    public DialogBoardViewModel()
        : this(
            DesignTime.CommandService,
            NullLoggerFactory.Instance,
            NullDialogService.Instance,
            NullNavigationService.Instance
        )
    {
        DesignTime.ThrowIfNotDesignMode();
        Title = RS.DialogPageViewModel_Title;
    }

    [ImportingConstructor]
    public DialogBoardViewModel(
        ICommandService cmd,
        ILoggerFactory loggerFactory,
        IDialogService dialogService,
        INavigationService navigationService
    )
        : base(PageId, cmd, loggerFactory)
    {
        Title = RS.DialogPageViewModel_Title;

        _loggerFactory = loggerFactory;
        _navigationService = navigationService;

        _selectFolderDialog = dialogService.GetDialogPrefab<SelectFolderDialogDesktopPrefab>();
        _observeFolderDialog = dialogService.GetDialogPrefab<ObserveFolderDialogPrefab>();
        _saveFileDialog = dialogService.GetDialogPrefab<SaveFileDialogDesktopPrefab>();
        _openFileDialog = dialogService.GetDialogPrefab<OpenFileDialogDesktopPrefab>();
        _saveCancelDialog = dialogService.GetDialogPrefab<SaveCancelDialogPrefab>();
        _yesNoDialog = dialogService.GetDialogPrefab<YesOrNoDialogPrefab>();
        _inputDialog = dialogService.GetDialogPrefab<InputDialogPrefab>(); // _positionDialog = dialogService.GetDialogPrefab<PositionDialogPrefab>();

        var PrefabTest = dialogService.GetDialogPrefab<PrefabTest>();

        OpenFileCommand = new ReactiveCommand(OpenFileAsync);
        SaveFileCommand = new ReactiveCommand(SaveFileAsync);
        SelectFolderCommand = new ReactiveCommand(SelectFolderAsync);
        ObserveFolderCommand = new ReactiveCommand(ObserveFolderAsync);
        YesNoCommand = new ReactiveCommand(YesNoMessageAsync);
        SaveCancelCommand = new ReactiveCommand(SaveCancelAsync);
        ShowUnitInputCommand = new ReactiveCommand(ShowUnitInputAsync);
        OpenPositionDialogCommand = new ReactiveCommand(ShowPositionDialog);
    }

    public ReactiveCommand OpenPositionDialogCommand { get; }
    public ReactiveCommand OpenFileCommand { get; }
    public ReactiveCommand SaveFileCommand { get; }
    public ReactiveCommand SelectFolderCommand { get; }
    public ReactiveCommand ObserveFolderCommand { get; }
    public ReactiveCommand YesNoCommand { get; }
    public ReactiveCommand SaveCancelCommand { get; }
    public ReactiveCommand ShowUnitInputCommand { get; }

    private async ValueTask OpenFileAsync(Unit unit, CancellationToken cancellationToken)
    {
        if (OperatingSystem.IsWindows() || OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
        {
            var payload = new OpenFileDialogPayload { Title = "Open File" };

            var result = await _openFileDialog.ShowDialogAsync(payload);
            Logger.LogInformation("OpenFile result = {result}", result);
        }
    }

    private async ValueTask SaveFileAsync(Unit unit, CancellationToken cancellationToken)
    {
        if (OperatingSystem.IsWindows() || OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
        {
            var payload = new SaveFileDialogPayload { Title = "Save File" };

            var result = await _saveFileDialog.ShowDialogAsync(payload);
            Logger.LogInformation("SaveFile result = {result}", result);
        }
    }

    private async ValueTask SelectFolderAsync(Unit unit, CancellationToken cancellationToken)
    {
        if (OperatingSystem.IsWindows() || OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
        {
            var payload = new SelectFolderDialogPayload { Title = "Select Folder File" };

            var result = await _selectFolderDialog.ShowDialogAsync(payload);
            Logger.LogInformation("SelectFolder result = {result}", result);
        }
    }

    private async ValueTask ObserveFolderAsync(Unit unit, CancellationToken cancellationToken)
    {
        if (OperatingSystem.IsWindows() || OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
        {
            var payload = new ObserveFolderDialogPayload
            {
                Title = "Observe Folder",
                DefaultPath = Environment.CurrentDirectory,
            };

            var result = await _observeFolderDialog.ShowDialogAsync(payload);
            Logger.LogInformation("ObserveFolder result = {result}", result);
        }
    }

    private async ValueTask YesNoMessageAsync(Unit unit, CancellationToken cancellationToken)
    {
        var payload = new YesOrNoDialogPayload
        {
            Title = "Предупреждение",
            Message = "Вы действительно хотите выйти?",
        };

        var res = await _yesNoDialog.ShowDialogAsync(payload);
        Logger.LogInformation("YesNo result = {res}", res);
    }

    private async ValueTask SaveCancelAsync(Unit unit, CancellationToken cancellationToken)
    {
        var payload = new SaveCancelDialogPayload { Title = "Сохранение", Message = "Сохранить?" };

        var res = await _saveCancelDialog.ShowDialogAsync(payload);
        Logger.LogInformation("SaveCancel result = {res}", res);
    }

    private async ValueTask ShowUnitInputAsync(Unit unit, CancellationToken cancellationToken)
    {
        var payload = new InputDialogPayload
        {
            Title = "Поиск",
            Message = "Введите значение файла",
        };

        var res = await _inputDialog.ShowDialogAsync(payload);
        Logger.LogInformation("UnitInput result = {res}", res);
    }

    private async ValueTask ShowPositionDialog(Unit unit, CancellationToken cancellationToken)
    {
        var payload = new PositionDialogPayload()
        {
            Title = "Поиск",
            Message = "Введите значение файла",
        };

        // var res = await _positionDialog.ShowDialogAsync(payload); // Logger.LogInformation("UnitInput result = {res}", res);
    }

    public override IEnumerable<IRoutable> GetRoutableChildren()
    {
        return [];
    }

    protected override void AfterLoadExtensions() { }

    public override IExportInfo Source => SystemModule.Instance;
}
