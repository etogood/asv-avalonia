using Asv.Common;
using Material.Icons;
using Microsoft.Extensions.Logging;
using R3;

namespace Asv.Avalonia;

public sealed class HotKeyViewModel : RoutableViewModel
{
    public const string ViewModelId = "hotkey";
    public const string EmptyHotKey = "-";
    private readonly ICommandService _svc;
    private readonly IDialogService _dialogService;

    public HotKeyViewModel(
        IRoutable parent,
        ICommandInfo command,
        ICommandService svc,
        IDialogService dialogService,
        ILoggerFactory loggerFactory
    )
        : base(new NavigationId(ViewModelId, command.Id), loggerFactory)
    {
        _svc = svc;
        _dialogService = dialogService;
        Parent = parent;
        Base = command;

        var hotkey = svc.GetHotKey(command.Id);
        var editedHotkey = new ReactiveProperty<string?>(
            hotkey == command.DefaultHotKey ? EmptyHotKey : hotkey
        );

        EditedHotKey = new HistoricalStringProperty(
            nameof(EditedHotKey),
            editedHotkey,
            loggerFactory,
            this
        );

        SyncConflict(EditedHotKey.ModelValue.Value);

        EditedHotKey.ModelValue.Subscribe(SyncConflict).DisposeItWith(Disposable);
        svc.OnHotKey.Subscribe(_ => SyncConflict(EditedHotKey.ModelValue.Value))
            .DisposeItWith(Disposable);
        EditedHotKey
            .ViewValue.Subscribe(value =>
                _svc.SetHotKey(
                    Base.Id,
                    value == EmptyHotKey ? null : HotKeyInfo.Parse(value ?? string.Empty)
                )
            )
            .DisposeItWith(Disposable);
    }

    #region Table columns

    public MaterialIconKind Icon => Base.Icon;
    public string Name => Base.Name;
    public string Description => Base.Description;
    public string Source => Base.Source.ModuleName;
    public string? DefaultHotKey => Base.DefaultHotKey;
    public HistoricalStringProperty EditedHotKey { get; }
    public BindableReactiveProperty<bool> HasConflict { get; } = new();

    #endregion

    public Selection NameSelection { get; private init; }
    public Selection DescriptionSelection { get; private init; }
    public ICommandInfo Base { get; }

    public async Task EditCommand()
    {
        var dialog = _dialogService.GetDialogPrefab<HotKeyCaptureDialogPrefab>();
        var payload = new HotKeyCaptureDialogPayload
        {
            Title = RS.HotKeyViewModel_HotKeyCaptureDialog_Title,
            Message = RS.HotKeyViewModel_HotKeyCaptureDialog_Message,
            CurrentHotKey = Base.DefaultHotKey,
        };

        var hotKey = await dialog.ShowDialogAsync(payload);
        if (hotKey is not null)
        {
            EditedHotKey.ViewValue.Value = hotKey;
        }
    }

    public static bool TryCreate(
        ICommandInfo info,
        IRoutable parent,
        ISearchService searchService,
        ICommandService commandsService,
        IDialogService dialogService,
        string? query,
        ILoggerFactory loggerFactory,
        out HotKeyViewModel? vm
    )
    {
        vm = null;
        var descriptionMatch = Selection.Empty;

        var result =
            searchService.Match(info.Name, query, out var nameMatch)
            || searchService.Match(info.Description, query, out descriptionMatch);
        if (!result)
        {
            return false;
        }

        vm = new HotKeyViewModel(parent, info, commandsService, dialogService, loggerFactory)
        {
            NameSelection = nameMatch,
            DescriptionSelection = descriptionMatch,
        };

        return true;
    }

    private void SyncConflict(string? value)
    {
        if (value == null)
        {
            HasConflict.Value = false;
            return;
        }

        var duplicateExists = _svc
            .Commands.Where(c => c.Id != Base.Id && EditedHotKey.ModelValue.Value != EmptyHotKey)
            .Select(c => _svc.GetHotKey(c.Id))
            .Any(hk => hk == value);

        HasConflict.Value = duplicateExists;
    }

    public override IEnumerable<IRoutable> GetRoutableChildren()
    {
        yield return EditedHotKey;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            HasConflict.Dispose();
            EditedHotKey.Dispose();
        }

        base.Dispose(disposing);
    }
}
