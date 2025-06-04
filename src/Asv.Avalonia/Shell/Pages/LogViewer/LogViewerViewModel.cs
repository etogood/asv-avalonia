using System.Composition;
using Asv.Common;
using Material.Icons;
using Microsoft.Extensions.Logging;
using ObservableCollections;
using R3;
using ZLogger;

namespace Asv.Avalonia.LogViewer;

public interface ILogViewerViewModel : IPage { }

[ExportPage(PageId)]
public class LogViewerViewModel : PageViewModel<ILogViewerViewModel>, ILogViewerViewModel
{
    private readonly ILogService _logService;
    private readonly ObservableList<LogMessageViewModel> _itemsSource = new();
    private readonly ReactiveProperty<string?> _filter;
    private string _fromToText;
    private readonly ILogger<LogViewerViewModel> _logger;
    private string _textMessage;
    private LogMessageViewModel _selectedItem;
    public const string PageId = "log";
    public const MaterialIconKind PageIcon = MaterialIconKind.Journal;

    public LogViewerViewModel()
        : base(DesignTime.Id, NullCommandService.Instance)
    {
        DesignTime.ThrowIfNotDesignMode();
        Title = "Log Viewer";
        Icon = PageIcon;

        _filter = new ReactiveProperty<string?>().DisposeItWith(Disposable);
        SearchText = new HistoricalStringProperty("search", _filter)
            .SetRoutableParent(this)
            .DisposeItWith(Disposable);

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
        ILoggerFactory loggerFactory
    )
        : base(PageId, cmd)
    {
        _logService = logService;
        Title = "Log Viewer";
        Icon = PageIcon;
        _logger = loggerFactory.CreateLogger<LogViewerViewModel>();
        Items = _itemsSource
            .ToNotifyCollectionChangedSlim(SynchronizationContextCollectionEventDispatcher.Current)
            .SetRoutableParent(this, Disposable)
            .DisposeItWith(Disposable);

        Start = new BindableReactiveProperty<int>(0).DisposeItWith(Disposable);
        Stop = new BindableReactiveProperty<int>(0).DisposeItWith(Disposable);
        Count = new BindableReactiveProperty<int>(50).DisposeItWith(Disposable);

        Update = new CancellableCommandWithProgress(
            UpdateImpl,
            $"{PageId}.refresh",
            loggerFactory
        ).DisposeItWith(Disposable);
        _filter = new ReactiveProperty<string?>().DisposeItWith(Disposable);
        SearchText = new HistoricalStringProperty("search", _filter).DisposeItWith(Disposable);
        SearchText.Parent = this;
        _filter.Skip(1).Subscribe(_ => Update.Command.Execute(null));
        Start.Subscribe(_ => Update.Command.Execute(null));

        Next = new ReactiveCommand(_ =>
        {
            Start.Value += Count.Value;
            Update.Command.Execute(null);
        }).DisposeItWith(Disposable);
        Previous = new ReactiveCommand(_ =>
        {
            var temp = Start.Value - Count.Value;
            Start.Value = temp < 0 ? 0 : temp;
            Update.Command.Execute(null);
        }).DisposeItWith(Disposable);
    }

    public CancellableCommandWithProgress Update { get; }

    public async Task UpdateImpl(IProgress<double> progress, CancellationToken cancel)
    {
        try
        {
            var text = _filter.Value?.ToLower();
            _itemsSource.Clear();
            var filtered = 0;
            var total = 0;
            var skip = 0;
            _logger.ZLogTrace($"Start filtering log messages with filter: '{text}'");
            await foreach (var logMessage in _logService.LoadItemsFromLogFile(cancel))
            {
                if (cancel.IsCancellationRequested)
                {
                    break;
                }

                ++total;

                if (ApplyFilter(logMessage, text))
                {
                    ++filtered;
                    if (filtered < Start.Value)
                    {
                        ++skip;
                        TextMessage = $"Skipped {skip}, filtered {filtered} messages from {total}";
                    }
                    else
                    {
                        _itemsSource.Add(new LogMessageViewModel(logMessage, this));
                        progress.Report((double)_itemsSource.Count / Count.Value);
                        TextMessage = $"Skipped {skip}, filtered {filtered} messages from {total}";
                    }

                    if (_itemsSource.Count >= Count.Value)
                    {
                        break;
                    }
                }
                else
                {
                    TextMessage = $"Skipped {skip}, filtered {filtered} messages from {total}";
                }
            }
        }
        finally
        {
            FromToText = $"{Start.Value + 1} - {Start.Value + _itemsSource.Count}";
            progress.Report(1);
        }
    }

    private static bool ApplyFilter(LogMessage logMessage, string? filter)
    {
        if (string.IsNullOrWhiteSpace(filter))
        {
            return true;
        }

        return logMessage.Message.ToLowerInvariant().Contains(filter)
            || logMessage.Category.Contains(filter)
            || logMessage.LogLevel.ToString().ToLowerInvariant().Contains(filter);
    }

    public BindableReactiveProperty<int> Start { get; }
    public IReadOnlyBindableReactiveProperty<int> Stop { get; }
    public BindableReactiveProperty<int> Count { get; }

    public HistoricalStringProperty SearchText { get; }

    public override IEnumerable<IRoutable> GetRoutableChildren()
    {
        yield return SearchText;
        foreach (var item in Items)
        {
            yield return item;
        }
    }

    public override ValueTask<IRoutable> Navigate(NavigationId id)
    {
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

    public string FromToText
    {
        get => _fromToText;
        set => SetField(ref _fromToText, value);
    }

    public string TextMessage
    {
        get => _textMessage;
        set => SetField(ref _textMessage, value);
    }

    public ReactiveCommand Next { get; }
    public ReactiveCommand Previous { get; }

    public LogMessageViewModel SelectedItem
    {
        get => _selectedItem;
        set => SetField(ref _selectedItem, value);
    }
}
