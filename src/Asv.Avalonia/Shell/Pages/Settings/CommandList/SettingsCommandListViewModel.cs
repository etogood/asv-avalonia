using System.Composition;
using Asv.Common;
using ObservableCollections;
using R3;

namespace Asv.Avalonia;

[ExportSettings(SubPageId)]
public class SettingsCommandListViewModel : SettingsSubPage
{
    public const string SubPageId = "settings.commandlist";
    private readonly ISynchronizedView<ICommandInfo, SettingsCommandListItemViewModel> _view;

    public SettingsCommandListViewModel()
        : this(DesignTime.CommandService)
    {
        DesignTime.ThrowIfNotDesignMode();
    }

    [ImportingConstructor]
    public SettingsCommandListViewModel(ICommandService svc)
        : base(SubPageId)
    {
        SearchText = new BindableReactiveProperty<string>().DisposeItWith(Disposable);
        var observableList = new ObservableList<ICommandInfo>(svc.Commands);
        observableList.Sort(CommandInfoKvpComparer.Instance);
        SelectedItem =
            new BindableReactiveProperty<SettingsCommandListItemViewModel?>().DisposeItWith(
                Disposable
            );

        _view = observableList
            .CreateView(x => new SettingsCommandListItemViewModel(x, svc))
            .DisposeItWith(Disposable);
        _view.DisposeMany().DisposeItWith(Disposable);
        _view.SetRoutableParentForView(this).DisposeItWith(Disposable);
        Items = _view.ToNotifyCollectionChanged().DisposeItWith(Disposable);
        SelectedItem
            .Subscribe(item =>
            {
                if (item is null)
                {
                    return;
                }

                _view.ForEach(it => it.IsSelected.Value = false);
                item.IsSelected.Value = true;
            })
            .DisposeItWith(Disposable);
        SearchText
            .ThrottleLast(TimeSpan.FromMilliseconds(500))
            .Subscribe(x =>
            {
                if (x.IsNullOrWhiteSpace())
                {
                    _view.ResetFilter();
                }
                else
                {
                    _view.AttachFilter((_, model) => model.Filter(x));
                }
            })
            .DisposeItWith(Disposable);
        ResetHotKeysToDefaultCommand = new ReactiveCommand(
            async (_, _) =>
            {
                var data = new ListCommandArg<(NavigationId, string?)>([]);
                await this.ExecuteCommand(ResetHotKeysCommand.Id, data);
            }
        ).DisposeItWith(Disposable);
    }

    public ReactiveCommand ResetHotKeysToDefaultCommand { get; set; }
    public NotifyCollectionChangedSynchronizedViewList<SettingsCommandListItemViewModel> Items { get; }
    public BindableReactiveProperty<string> SearchText { get; }
    public BindableReactiveProperty<SettingsCommandListItemViewModel?> SelectedItem { get; }

    public override ValueTask<IRoutable> Navigate(NavigationId id)
    {
        var item = _view.FirstOrDefault(x => x.Id == id);
        if (item != null)
        {
            SelectedItem.Value = item;
            return ValueTask.FromResult<IRoutable>(item);
        }

        return base.Navigate(id);
    }

    public override IEnumerable<IRoutable> GetRoutableChildren()
    {
        foreach (var child in base.GetRoutableChildren())
        {
            yield return child;
        }

        foreach (var model in _view)
        {
            yield return model;
        }
    }

    public override IExportInfo Source => SystemModule.Instance;
}

file class CommandInfoKvpComparer : IComparer<ICommandInfo>
{
    public static CommandInfoKvpComparer Instance { get; } = new();

    private CommandInfoKvpComparer() { }

    public int Compare(ICommandInfo? x, ICommandInfo? y)
    {
        return string.CompareOrdinal(x?.Id, y?.Id);
    }
}
