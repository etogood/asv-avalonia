using Asv.Common;
using Microsoft.Extensions.Logging;
using R3;

namespace Asv.Avalonia;

public class SearchBoxViewModel : RoutableViewModel
{
    private readonly CommandExecuteDelegate<string> _searchCallback;
    private bool _isSelected;
    private string _text = string.Empty;
    private string _previousTextSearch = string.Empty;

    public SearchBoxViewModel()
        : this(DesignTime.Id, DesignTime.LoggerFactory, (_, _, _) => Task.CompletedTask)
    {
        DesignTime.ThrowIfNotDesignMode();
    }

    public SearchBoxViewModel(
        string id,
        ILoggerFactory loggerFactory,
        CommandExecuteDelegate<string> searchCallback,
        TimeSpan? throttleTime = null
    )
        : base(id)
    {
        _searchCallback = searchCallback;
        Search = new CancellableCommandWithProgress<string>(
            InternalSearchCallback,
            "Search",
            loggerFactory
        ).DisposeItWith(Disposable);

        UpdateCommand = new ReactiveCommand(_ => Update()).DisposeItWith(Disposable);

        if (throttleTime != null)
        {
            this.ObservePropertyChanged(x => x.Text)
                .Skip(1)
                .ThrottleLast(throttleTime.Value)
                .Subscribe(x => TextSearchCommand.ExecuteCommand(this, x))
                .DisposeItWith(Disposable);
        }

        Clear = new ReactiveCommand(_ => Text = string.Empty).DisposeItWith(Disposable);
    }

    private Task InternalSearchCallback(
        string arg,
        IProgress<double> progress,
        CancellationToken cancel
    )
    {
        _previousTextSearch = arg;
        Text = arg;
        return _searchCallback(arg, progress, cancel);
    }

    public string Text
    {
        get => _text;
        set => SetField(ref _text, value);
    }

    public CancellableCommandWithProgress<string> Search { get; }
    public ReactiveCommand Clear { get; }

    public bool IsSelected
    {
        get => _isSelected;
        set => SetField(ref _isSelected, value);
    }

    public string PreviousTextSearch => _previousTextSearch;

    public override IEnumerable<IRoutable> GetRoutableChildren()
    {
        yield break;
    }

    public override ValueTask<IRoutable> Navigate(NavigationId id)
    {
        IsSelected = false;
        IsSelected = true;
        return base.Navigate(id);
    }

    public ReactiveCommand UpdateCommand { get; }

    public void ForceUpdateWithoutHistory()
    {
        // This method is used to force an update without adding to the history.
        // It can be useful when you want to refresh the search results without
        // changing the current state of the application.
        Search.Command.Execute(Text);
    }

    public void Update()
    {
        TextSearchCommand.ExecuteCommand(this, Text);
    }
}
