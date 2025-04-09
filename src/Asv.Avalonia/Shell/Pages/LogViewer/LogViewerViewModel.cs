using System.Composition;
using Asv.Avalonia.Comparers;
using Asv.Common;
using Material.Icons;
using Microsoft.Extensions.Logging;
using ObservableCollections;
using R3;

namespace Asv.Avalonia;

[ExportPage(PageId)]
public class LogViewerViewModel : PageViewModel<LogViewerViewModel>, IPage
{
    public const string PageId = "logviewer";
    private const int MinPageIndex = 1;
    public const MaterialIconKind PageIcon = MaterialIconKind.Journal;

    private readonly ILogService _log;
    private readonly IDialogService _dialogService;

    private readonly HashSet<LogLevel> _logLevels;
    private readonly HashSet<string> _logCategories;
    private readonly HashSet<string> _logThreadIds;

    private readonly List<LogItemViewModel> _logItems;
    private readonly ObservableList<LogItemViewModel> _writableLogItems;

    [ImportingConstructor]
    public LogViewerViewModel(ICommandService cmd, ILogService log, IDialogService dialogService)
        : base(PageId, cmd)
    {
        _dialogService = dialogService;
        _log = log;

        Icon.OnNext(MaterialIconKind.Journal);
        Title.OnNext(RS.LogViewer_Title);

        _logLevels = new HashSet<LogLevel>();
        _logCategories = new HashSet<string>();
        _logThreadIds = new HashSet<string>();

        _logItems = new List<LogItemViewModel>();
        PageSize = new BindableReactiveProperty<int>(PageSizes[0]);
        FilteredItemsCount = new BindableReactiveProperty<int>();
        TotalItemsCount = new BindableReactiveProperty<int>(_logItems.Count);
        TotalPagesCount = new BindableReactiveProperty<int>(TotalItemsCount.Value / PageSize.Value);
        TotalItemsCount.Subscribe(_ => TotalPagesCount.Value = _ / PageSize.Value);
        SelectedLevels = new ObservableList<LogLevel>()
            .CreateWritableView(x => x)
            .ToWritableNotifyCollectionChanged();
        SelectedCategories = new ObservableList<string>()
            .CreateWritableView(x => x)
            .ToWritableNotifyCollectionChanged();
        SelectedThreadIds = new ObservableList<string>()
            .CreateWritableView(x => x)
            .ToWritableNotifyCollectionChanged();
        AvailableLevels.ForEach(level => SelectedLevels.Add(level));
        AvailableCategories.ForEach(cls => SelectedCategories.Add(cls));
        AvailableThreadIds.ForEach(thread => SelectedThreadIds.Add(thread));
        SearchText = new BindableReactiveProperty<string>();
        CurrentPageIndex = new BindableReactiveProperty<int>();
        SearchText.Subscribe(_ => CurrentPageIndex.OnNext(1));
        MoveToPreviousPageCommand = new ReactiveCommand(_ => MoveToPreviousPage());
        MoveToNextPageCommand = new ReactiveCommand(_ => MoveToNextPage());
        ClearSearchTextCommand = new ReactiveCommand(_ => SearchText.OnNext(string.Empty));
        SearchText.Subscribe(text =>
            ClearSearchTextCommand.ChangeCanExecute(!string.IsNullOrEmpty(text))
        );

        SelectAllThreadIdsCommand = new ReactiveCommand(_ =>
            AvailableThreadIds.ForEach(thread => SelectedThreadIds.Add(thread))
        );
        DeselectAllThreadIdsCommand = new ReactiveCommand(_ => SelectedThreadIds.Clear());

        SelectAllCategoriesCommand = new ReactiveCommand(_ =>
            AvailableCategories.ForEach(cls => SelectedCategories.Add(cls))
        );
        DeselectAllCategoriesCommand = new ReactiveCommand(_ => SelectedCategories.Clear());

        SelectAllLevelsCommand = new ReactiveCommand(_ =>
            AvailableLevels.ForEach(level => SelectedLevels.Add(level))
        );
        DeselectAllLevelsCommand = new ReactiveCommand(_ => SelectedLevels.Clear());

        ClearLogsCommand = new ReactiveCommand(ClearLogs);
        _log.LoadItemsFromLogFile()
            .ForEach(x => _logItems.Add(new LogItemViewModel(x.Timestamp.Ticks, x)));
        _logItems.Select(_ => _.Level).ForEach(level => _logLevels.Add(level));
        _logItems.Select(_ => _.Category).ForEach(category => _logCategories.Add(category));
        TotalItemsCount.Value = _logItems.Count;
        _log.OnMessage.Subscribe(message =>
        {
            if (message is not null)
            {
                _logItems.Add(new LogItemViewModel(message.Timestamp.Ticks, message));
            }
        });
        _writableLogItems = new ObservableList<LogItemViewModel>(_logItems);
        var logView = _writableLogItems.CreateWritableView(x => x);
        logView.DisposeRemovedViewItems();
       
         logView.AttachFilter(FilterItem);
        _writableLogItems.SetRoutableParent(this, false);
        _writableLogItems.Sort(LogItemDescendingComparer.Instance);
        LogItemsView = logView.ToWritableNotifyCollectionChanged();
        LogItemsView.SyncCollection(
            logView.Skip(PageSize.Value * CurrentPageIndex.Value).Take(PageSize.Value),
            a => LogItemsView.Remove(a), 
            x => LogItemsView.Add(x), LogItemDescendingComparer.Instance);
        Observable
            .CombineLatest(PageSize, CurrentPageIndex)
            .Subscribe(tuple =>
            {
                var pageSize = tuple[0];
                var currentIndex = tuple[1];

                if (FilteredItemsCount.Value != 0)
                {
                    TotalPagesCount.OnNext(FilteredItemsCount.Value / pageSize);
                }

                if (currentIndex < MinPageIndex)
                {
                    CurrentPageIndex.OnNext(MinPageIndex);
                }

                if (currentIndex > TotalPagesCount.Value)
                {
                    CurrentPageIndex.OnNext(TotalPagesCount.Value);
                }

                if (_writableLogItems.Count != 0 )
                {
                    _writableLogItems.Clear();     
                }
               
                _writableLogItems.AddRange(_logItems.Skip(PageSize.Value * CurrentPageIndex.Value).Take(PageSize.Value));
            });
    }

    public static List<int> PageSizes => [25, 50, 100, 200];
    public IEnumerable<LogLevel> AvailableLevels => _logLevels;
    public IEnumerable<string> AvailableCategories => _logCategories;
    public IEnumerable<string> AvailableThreadIds => _logThreadIds;
    public BindableReactiveProperty<int> TotalItemsCount { get; }
    public BindableReactiveProperty<int> FilteredItemsCount { get; }
    public BindableReactiveProperty<string> SearchText { get; }
    public BindableReactiveProperty<int> CurrentPageIndex { get; }
    public BindableReactiveProperty<int> TotalPagesCount { get; }
    public BindableReactiveProperty<int> PageSize { get; }
    public NotifyCollectionChangedSynchronizedViewList<LogItemViewModel> LogItemsView { get; set; }
    public NotifyCollectionChangedSynchronizedViewList<LogLevel> SelectedLevels { get; set; }
    public NotifyCollectionChangedSynchronizedViewList<string> SelectedCategories { get; set; }
    public NotifyCollectionChangedSynchronizedViewList<string> SelectedThreadIds { get; set; }
    public ReactiveCommand SelectAllThreadIdsCommand { get; }
    public ReactiveCommand DeselectAllThreadIdsCommand { get; }
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
        FilteredItemsCount.Value = TotalItemsCount.Value;

        // var containsLevel = SelectedLevels.Contains(item.Level);
        // var containsClass = SelectedCategories.Contains(item.Category);
        return containsMessage;

        // && containsLevel && containsClass;
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