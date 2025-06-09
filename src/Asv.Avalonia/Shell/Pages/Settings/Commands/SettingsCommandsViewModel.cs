using System.Composition;
using Asv.Common;
using ObservableCollections;
using R3;

namespace Asv.Avalonia;

[ExportSettings(SubPageId)]
public class SettingsCommandsViewModel : SettingsSubPage
{
    public const string SubPageId = "commands";
    private readonly ObservableList<ICommandInfo> _itemsSource;
    private readonly ISynchronizedView<ICommandInfo, SettingsCommandsItemViewModel> _itemsView;

    // This is a common constructor for design time and runtime.
    private SettingsCommandsViewModel(string id, ICommandService svc)
        : base(id)
    {
        _itemsSource = [];
        _itemsView = _itemsSource
            .CreateView(x => new SettingsCommandsItemViewModel(x, svc))
            .DisposeMany(Disposable)
            .SetRoutableParent(this, Disposable)
            .DisposeItWith(Disposable);

        Items = _itemsView
            .ToNotifyCollectionChanged(SynchronizationContextCollectionEventDispatcher.Current)
            .DisposeItWith(Disposable);

        SelectedItem = new BindableReactiveProperty<SettingsCommandsItemViewModel>().DisposeItWith(
            Disposable
        );

        var filter = new ReactiveProperty<string?>().DisposeItWith(Disposable);
        SearchText = new HistoricalStringProperty("search", filter)
            .SetRoutableParent(this)
            .DisposeItWith(Disposable);

        filter.Skip(1).Subscribe(AttachFilter).DisposeItWith(Disposable);
    }

    private void AttachFilter(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            _itemsView.ResetFilter();
        }
        else
        {
            _itemsView.AttachFilter((_, vm) => vm.Filter(text));
        }
    }

    public SettingsCommandsViewModel()
        : this(DesignTime.Id, DesignTime.CommandService)
    {
        DesignTime.ThrowIfNotDesignMode();
        _itemsSource.Add(UndoCommand.StaticInfo);
        _itemsSource.Add(RedoCommand.StaticInfo);
        _itemsSource.Add(OpenSettingsCommand.StaticInfo);
    }

    [ImportingConstructor]
    public SettingsCommandsViewModel(ICommandService svc)
        : this(SubPageId, svc)
    {
        _itemsSource.AddRange(svc.Commands.OrderBy(x => x.Name));
    }

    public NotifyCollectionChangedSynchronizedViewList<SettingsCommandsItemViewModel> Items { get; }
    public HistoricalStringProperty SearchText { get; }
    public BindableReactiveProperty<SettingsCommandsItemViewModel> SelectedItem { get; }
    public override IExportInfo Source => SystemModule.Instance;

    public override IEnumerable<IRoutable> GetRoutableChildren()
    {
        foreach (var child in base.GetRoutableChildren())
        {
            yield return child;
        }

        yield return SearchText;
    }
}
