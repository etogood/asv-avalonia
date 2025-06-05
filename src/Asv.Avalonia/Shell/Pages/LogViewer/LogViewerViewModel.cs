using System.Composition;
using Asv.Common;
using Avalonia.Input;
using Avalonia.Threading;
using Material.Icons;
using Microsoft.Extensions.Logging;
using ObservableCollections;
using R3;
using ZLogger;

namespace Asv.Avalonia.LogViewer;

public interface ILogViewerViewModel : IPage { }

[ExportPage(PageId)]
public class LogViewerViewModel
    : PageViewModel<ILogViewerViewModel>,
        ILogViewerViewModel,
        ISupportPagination
{
    private readonly ILogService _logService;
    private readonly ISearchService _search;
    private readonly ObservableList<LogMessageViewModel> _itemsSource = new();
    private readonly ILogger<LogViewerViewModel> _logger;
    private LogMessageViewModel _selectedItem;
    public const string PageId = "log";
    public const MaterialIconKind PageIcon = MaterialIconKind.Journal;

    public LogViewerViewModel()
        : base(DesignTime.Id, NullCommandService.Instance)
    {
        DesignTime.ThrowIfNotDesignMode();
        Title = "Log Viewer";
        Icon = PageIcon;
        Filter = new SearchBoxViewModel();
        Items = _itemsSource
            .ToNotifyCollectionChangedSlim(SynchronizationContextCollectionEventDispatcher.Current)
            .DisposeItWith(Disposable);

        _itemsSource.Add(
            new LogMessageViewModel(
                new LogMessage(
                    DateTime.Now,
                    LogLevel.Error,
                    "DesignTime",
                    "Design time log message",
                    "This is a design time log message for the Log Viewer. It will not be shown in the actual application."
                ),
                this
            )
        );
        _itemsSource.Add(
            new LogMessageViewModel(
                new LogMessage(
                    DateTime.Now,
                    LogLevel.Information,
                    "asd.asd.asd.a.dsasd",
                    "Design time log message 2",
                    "This is another design time log message for the Log Viewer. It will not be shown in the actual application."
                ),
                this
            )
        );
        _itemsSource.Add(
            new LogMessageViewModel(
                new LogMessage(
                    DateTime.Now,
                    LogLevel.Warning,
                    "DesignTime",
                    "Design time log message 3",
                    "This is yet another design time log message for the Log Viewer. It will not be shown in the actual application."
                ),
                this
            )
        );
        _itemsSource.Add(
            new LogMessageViewModel(
                new LogMessage(
                    DateTime.Now,
                    LogLevel.Debug,
                    "asdasdasdasdasd",
                    "Design time log message 4",
                    "This is a debug log message for the Log Viewer. It will not be shown in the actual application."
                ),
                this
            )
        );
        _itemsSource.Add(
            new LogMessageViewModel(
                new LogMessage(
                    DateTime.Now,
                    LogLevel.Critical,
                    "DesignTime",
                    "Design time log message 5",
                    "This is a critical log message for the Log Viewer. It will not be shown in the actual application."
                ),
                this
            )
        );
        _itemsSource.Add(
            new LogMessageViewModel(
                new LogMessage(
                    DateTime.Now,
                    LogLevel.Trace,
                    "asdasdasd",
                    "Design time log message 6",
                    "This is a trace log message for the Log Viewer. It will not be shown in the actual application."
                ),
                this
            )
        );
        _itemsSource.Add(
            new LogMessageViewModel(
                new LogMessage(
                    DateTime.Now,
                    LogLevel.None,
                    "DesignTime asdasd",
                    "Design time log message 7",
                    "This is a log message with no specific level for the Log Viewer. It will not be shown in the actual application."
                ),
                this
            )
        );
    }

    [ImportingConstructor]
    public LogViewerViewModel(
        ICommandService cmd,
        ILogService logService,
        ISearchService search,
        ILoggerFactory loggerFactory
    )
        : base(PageId, cmd)
    {
        _logService = logService;
        _search = search;
        Title = "Log Viewer";
        Icon = PageIcon;
        _logger = loggerFactory.CreateLogger<LogViewerViewModel>();
        Items = _itemsSource
            .ToNotifyCollectionChangedSlim()
            .SetRoutableParent(this, Disposable)
            .DisposeItWith(Disposable);

        Skip = new BindableReactiveProperty<int>(0).DisposeItWith(Disposable);
        Stop = new BindableReactiveProperty<int>(0).DisposeItWith(Disposable);
        Take = new BindableReactiveProperty<int>(50).DisposeItWith(Disposable);
        FromToText = new BindableReactiveProperty<string>(string.Empty).DisposeItWith(Disposable);
        TextMessage = new BindableReactiveProperty<string>(string.Empty).DisposeItWith(Disposable);

        Filter = new SearchBoxViewModel("search", loggerFactory, UpdateImpl)
            .SetRoutableParent(this)
            .DisposeItWith(Disposable);

        Skip.Skip(1).Subscribe(_ => Filter.ForceUpdateWithoutHistory());

        Next = new ReactiveCommand(_ =>
        {
            PaginationCommand.Execute(this, Skip.Value + Take.Value, Take.Value);
        }).DisposeItWith(Disposable);
        Previous = new ReactiveCommand(_ =>
        {
            var temp = Skip.Value - Take.Value;
            PaginationCommand.Execute(this, temp < 0 ? 0 : temp, Take.Value);
        }).DisposeItWith(Disposable);

        Filter.ForceUpdateWithoutHistory();
    }

    public SearchBoxViewModel Filter { get; }

    public async Task UpdateImpl(
        string? query,
        IProgress<double> progress,
        CancellationToken cancel
    )
    {
        try
        {
            var text = query?.ToLower();
            Dispatcher.UIThread.Invoke(_itemsSource.Clear);

            var filtered = 0;
            var total = 0;
            var skip = 0;
            _logger.ZLogTrace($"Start filtering log messages with filter: '{text}'");
            progress.Report(double.NaN);

            await foreach (var logMessage in _logService.LoadItemsFromLogFile(cancel))
            {
                if (cancel.IsCancellationRequested)
                {
                    break;
                }

                ++total;

                if (_search.Match(logMessage, text))
                {
                    ++filtered;
                    if (filtered < Skip.Value)
                    {
                        ++skip;
                        TextMessage.Value =
                            $"Skipped {skip}, filtered {filtered} messages from {total}";
                    }
                    else
                    {
                        Dispatcher.UIThread.Invoke(
                            () => _itemsSource.Add(new LogMessageViewModel(logMessage, this))
                        );
                        progress.Report((double)_itemsSource.Count / Take.Value);
                        TextMessage.Value =
                            $"Skipped {skip}, filtered {filtered} messages from {total}";
                    }

                    if (_itemsSource.Count >= Take.Value)
                    {
                        break;
                    }
                }
                else
                {
                    TextMessage.Value =
                        $"Skipped {skip}, filtered {filtered} messages from {total}";
                }
            }
        }
        finally
        {
            FromToText.Value = $"{Skip.Value + 1} - {Skip.Value + _itemsSource.Count}";
            progress.Report(1);
        }
    }

    public BindableReactiveProperty<int> Skip { get; }
    public IReadOnlyBindableReactiveProperty<int> Stop { get; }
    public BindableReactiveProperty<int> Take { get; }

    public override IEnumerable<IRoutable> GetRoutableChildren()
    {
        yield return Filter;
        foreach (var item in Items)
        {
            yield return item;
        }
    }

    public override ValueTask<IRoutable> Navigate(NavigationId id)
    {
        if (id == Filter.Id)
        {
            Filter.Navigate(NavigationId.Empty);
            return new ValueTask<IRoutable>(Filter);
        }

        var item = Items.FirstOrDefault(x => x.Id == id);
        if (item == null)
        {
            return ValueTask.FromResult<IRoutable>(this);
        }

        SelectedItem = item;
        return ValueTask.FromResult<IRoutable>(item);
    }

    protected override void AfterLoadExtensions() { }

    public override IExportInfo Source => SystemModule.Instance;
    public INotifyCollectionChangedSynchronizedViewList<LogMessageViewModel> Items { get; }

    public BindableReactiveProperty<string> FromToText { get; }
    public BindableReactiveProperty<string> TextMessage { get; }

    public ReactiveCommand Next { get; }
    public ReactiveCommand Previous { get; }

    public LogMessageViewModel SelectedItem
    {
        get => _selectedItem;
        set => SetField(ref _selectedItem, value);
    }
}
