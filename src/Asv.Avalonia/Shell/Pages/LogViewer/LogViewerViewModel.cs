using System.Composition;
using Asv.Avalonia.Comparers;
using Asv.Common;
using Avalonia.Controls;
using Material.Icons;
using Microsoft.Extensions.Logging;
using ObservableCollections;
using R3;

namespace Asv.Avalonia;

public sealed class LogViewerFilterItem(string name, bool isSelected)
{
    public string Name { get; set; } = name;
    public BindableReactiveProperty<bool> IsSelected { get; } = new(isSelected);
}

[ExportPage(PageId)]
public class LogViewerViewModel : PageViewModel<LogViewerViewModel>, IPage
{
    public const string PageId = "logviewer";
    private const int MinPageIndex = 1;
    public const MaterialIconKind PageIcon = MaterialIconKind.Journal;

    private readonly ILogService _log;
    private readonly IDialogService _dialogService;

    private readonly ObservableList<LogViewerFilterItem> _logLevels = [];
    private readonly ObservableList<LogViewerFilterItem> _logCategories = [];

    private readonly List<LogItemViewModel> _logItems;
    private readonly ObservableList<LogItemViewModel> _writableLogItems;

    public LogViewerViewModel() : base(DesignTime.Id, NullCommandService.Instance)
    {
        if (Design.IsDesignMode)
        {
            IsFiltersVisible.Value = false;
            _logCategories = new ObservableList<LogViewerFilterItem>()
            {
                new("Category 0", true),
                new("Category 1", true),
                new("Category 2", false),
            };
            _logLevels = new ObservableList<LogViewerFilterItem>()
            {
                new("Warning", true),
                new("Error", true),
                new("Information", true),
            };
            AvailableLevels = _logLevels.ToNotifyCollectionChangedSlim();
            AvailableCategories = _logCategories.ToNotifyCollectionChangedSlim();       
        }
    }

    [ImportingConstructor]
    public LogViewerViewModel(ICommandService cmd, ILogService log, IDialogService dialogService)
        : base(PageId, cmd)
    {
        _dialogService = dialogService;
        _log = log;

        Icon.OnNext(MaterialIconKind.Journal);
        Title.OnNext(RS.LogViewer_Title);

        _logItems = new List<LogItemViewModel>();
        PageSize = new BindableReactiveProperty<int>(PageSizes[0]);
        FilteredItemsCount = new BindableReactiveProperty<int>();
        TotalItemsCount = new BindableReactiveProperty<int>(_logItems.Count);
        TotalPagesCount = new BindableReactiveProperty<int>(TotalItemsCount.Value / PageSize.Value);
        TotalItemsCount.Subscribe(i => TotalPagesCount.Value = i / PageSize.Value);
        SelectedLevels = new ObservableList<LogLevel>();
        SelectedCategories = new ObservableList<string>();
        SearchText = new BindableReactiveProperty<string>();
        CurrentPageIndex = new BindableReactiveProperty<int>();
        SearchText.Subscribe(_ => CurrentPageIndex.OnNext(1)).DisposeItWith(Disposable);
        MoveToPreviousPageCommand = new ReactiveCommand(_ => MoveToPreviousPage()).DisposeItWith(Disposable);
        MoveToNextPageCommand = new ReactiveCommand(_ => MoveToNextPage()).DisposeItWith(Disposable);
        ClearSearchTextCommand = new ReactiveCommand(_ => SearchText.OnNext(string.Empty)).DisposeItWith(Disposable);
        SearchText.Subscribe(text => ClearSearchTextCommand.ChangeCanExecute(!string.IsNullOrEmpty(text))).DisposeItWith(Disposable);

        SelectAllCategoriesCommand = new ReactiveCommand(_ => _logCategories.ForEach(cls => cls.IsSelected.Value = true)).DisposeItWith(Disposable);
        DeselectAllCategoriesCommand = new ReactiveCommand(_ => _logCategories.ForEach(cls => cls.IsSelected.Value = false)).DisposeItWith(Disposable);

        SelectAllLevelsCommand = new ReactiveCommand(_ => _logLevels.ForEach(level => level.IsSelected.Value = true)).DisposeItWith(Disposable);
        DeselectAllLevelsCommand = new ReactiveCommand(_ => _logLevels.ForEach(level => level.IsSelected.Value = false)).DisposeItWith(Disposable);

        ClearLogsCommand = new ReactiveCommand(ClearLogs).DisposeItWith(Disposable);
        _log.LoadItemsFromLogFile()
            .ForEach(x => _logItems.Add(new LogItemViewModel(x.Timestamp.Ticks, x)));
        _logItems.Select(viewModel => viewModel.Level).ForEach(level =>
        {
            if (_logLevels.FirstOrDefault(item => item.Name == level.ToString()) is null)
            {
                _logLevels.Add(new LogViewerFilterItem(level.ToString(), true));
            }
        });
        _logItems.Select(viewModel => viewModel.Category)
            .ForEach(category =>
            {
                if (_logCategories.FirstOrDefault(item => item.Name == category) is null)
                {
                    _logCategories.Add(new LogViewerFilterItem(category, true));
                }
            });
        AvailableCategories = _logCategories.ToNotifyCollectionChangedSlim().DisposeItWith(Disposable);
        AvailableLevels = _logLevels.ToNotifyCollectionChangedSlim().DisposeItWith(Disposable);
        TotalItemsCount.Value = _logItems.Count;
        _log.OnMessage.Subscribe(message =>
        {
            if (message is not null)
            {
                _logItems.Add(new LogItemViewModel(message.Timestamp.Ticks, message));
            }
        }).DisposeItWith(Disposable);
        _writableLogItems = new ObservableList<LogItemViewModel>();
        var logView = _writableLogItems.CreateWritableView(x => x).DisposeItWith(Disposable);
        logView.DisposeRemovedViewItems().DisposeItWith(Disposable);
        logView.AttachFilter(FilterItem);
        _writableLogItems.SetRoutableParent(this, false);
        
        LogItemsView = logView.ToWritableNotifyCollectionChanged().DisposeItWith(Disposable);
        
        ApplyFilters = new ReactiveCommand(_ =>
        {
            CurrentPageIndex.OnNext(1);
            UpdateFilteredItems();
        }).DisposeItWith(Disposable);

        Observable
            .CombineLatest(PageSize, CurrentPageIndex)
            .Subscribe(tuple =>
            {
                var currentIndex = tuple[1];
                
                UpdateFilteredItems();
                
                if (currentIndex < MinPageIndex)
                {
                    CurrentPageIndex.OnNext(MinPageIndex);
                }

                if (currentIndex > TotalPagesCount.Value)
                {
                    CurrentPageIndex.OnNext(TotalPagesCount.Value);
                }
            }).DisposeItWith(Disposable);
    }
    
    private void UpdateFilteredItems()
    {
        var filteredItems = _logItems.Where(item =>
        {
            var containsMessage =
                string.IsNullOrEmpty(SearchText.Value)
                || item.Message.Contains(SearchText.Value, StringComparison.CurrentCultureIgnoreCase);
            
            var isSelectedLevel = _logLevels.FirstOrDefault(filterItem => filterItem.Name == item.Level.ToString())?.IsSelected.Value ?? false;
            var isSelectedCategory =
                _logCategories.FirstOrDefault(filterItem => filterItem.Name == item.Category)?.IsSelected.Value ?? false;
            
            return containsMessage && isSelectedLevel && isSelectedCategory;
        }).OrderByDescending(x => x, LogItemDescendingComparer.Instance).ToList();

        FilteredItemsCount.Value = filteredItems.Count;
        TotalPagesCount.Value = (int)Math.Ceiling((double)FilteredItemsCount.Value / PageSize.Value);

        var skip = (CurrentPageIndex.Value - 1) * PageSize.Value;
        var pageItems = filteredItems.Skip(skip).Take(PageSize.Value).ToList();

        _writableLogItems.Clear();
        _writableLogItems.AddRange(pageItems);
    }

    public static List<int> PageSizes => [25, 50, 100, 200];
    public NotifyCollectionChangedSynchronizedViewList<LogViewerFilterItem> AvailableLevels { get; set; }
    public ReactiveCommand ApplyFilters { get; }
    public NotifyCollectionChangedSynchronizedViewList<LogViewerFilterItem> AvailableCategories { get; set; }
    public BindableReactiveProperty<int> TotalItemsCount { get; }
    public BindableReactiveProperty<int> FilteredItemsCount { get; }
    public BindableReactiveProperty<string> SearchText { get; }
    public BindableReactiveProperty<bool> IsFiltersVisible { get; } = new();
    public BindableReactiveProperty<int> CurrentPageIndex { get; }
    public BindableReactiveProperty<int> TotalPagesCount { get; }
    public BindableReactiveProperty<int> PageSize { get; }
    public NotifyCollectionChangedSynchronizedViewList<LogItemViewModel> LogItemsView { get; set; }
    public ObservableList<LogLevel> SelectedLevels { get; set; }
    public ObservableList<string> SelectedCategories { get; set; }
    public ReactiveCommand SelectAllCategoriesCommand { get; }
    public ReactiveCommand DeselectAllCategoriesCommand { get; }
    public ReactiveCommand SelectAllLevelsCommand { get; }
    public ReactiveCommand DeselectAllLevelsCommand { get; }
    public ReactiveCommand MoveToPreviousPageCommand { get; }
    public ReactiveCommand MoveToNextPageCommand { get; }
    public ReactiveCommand ClearSearchTextCommand { get; }
    public ReactiveCommand ClearLogsCommand { get; }

    private bool FilterItem(LogItemViewModel item)
    {
        var containsMessage =
            string.IsNullOrEmpty(SearchText.Value)
            || item.Message.Contains(SearchText.Value, StringComparison.CurrentCultureIgnoreCase);
        
        var isSelectedLevel = _logLevels.FirstOrDefault(filterItem => filterItem.Name == item.Level.ToString())?.IsSelected.Value ?? false;
        var isSelectedCategory =
            _logCategories.FirstOrDefault(filterItem => filterItem.Name == item.Category)?.IsSelected.Value ?? false;
        
        return containsMessage && isSelectedLevel && isSelectedCategory;
    }

    private void MoveToPreviousPage()
    {
        CurrentPageIndex.OnNext(CurrentPageIndex.Value - 1);
    }

    private void MoveToNextPage()
    {
        CurrentPageIndex.OnNext(CurrentPageIndex.Value + 1);
    }

    private async ValueTask ClearLogs(Unit unit, CancellationToken ct)
    {
        var result = await _dialogService.ShowYesNoDialog(RS.LogViewerViewModel_ClearDialog_Title,
            RS.LogViewerViewModel_ClearDialog_Content);
        if (result)
        {
            _logItems.Clear();
            _log.DeleteLogFile();
            _log.Warning(nameof(LogViewerViewModel), "Log have been cleared!");
        }
    }

    public override IEnumerable<IRoutable> GetRoutableChildren()
    {
        return _logItems;
    }

    protected override void AfterLoadExtensions()
    {
    }

    public override IExportInfo Source => SystemModule.Instance;
}