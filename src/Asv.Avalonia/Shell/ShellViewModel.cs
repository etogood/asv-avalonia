using System.Collections.Immutable;
using System.Composition.Hosting;
using Avalonia;
using ObservableCollections;
using R3;

namespace Asv.Avalonia;

public abstract class ShellViewModel : RoutableViewModel, IShell
{
    
    private readonly ObservableList<IPage> _pages = new();
    private readonly IContainerHost _container;
    private Stack<string[]> _backwardStack = new();
    private Stack<string[]> _forwardStack = new();
    public const string ShellId = "shell";

    protected ShellViewModel(IContainerHost ioc)
        : base(ShellId)
    {
        ArgumentNullException.ThrowIfNull(ioc);
        _container = ioc;
        Pages = _pages.ToNotifyCollectionChangedSlim();
        Back = new ReactiveCommand((_, c) => BackwardAsync(c));
        Forward = new ReactiveCommand((_, c) => ForwardAsync(c));
        GoHome = new ReactiveCommand((_, c) => BackwardAsync(c));
        Status = new BindableReactiveProperty<ShellStatus>(ShellStatus.Normal);
        Close = new ReactiveCommand((_, c) => CloseAsync(c));
    }

    protected virtual ValueTask CloseAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public ReactiveCommand Back { get; }
    public async ValueTask BackwardAsync(CancellationToken cancel = default)
    {
        if (_backwardStack.TryPop(out var path))
        {
            _forwardStack.Push(path);
            await NavigateTo(path);
            CheckBackwardForwardCanExecute();
        }
    }

    private void CheckBackwardForwardCanExecute()
    {
        Back.ChangeCanExecute(_backwardStack.Count != 0);
        Forward.ChangeCanExecute(_forwardStack.Count != 0);
    }

    public ReactiveCommand Forward { get; }
    public async ValueTask ForwardAsync(CancellationToken cancel = default)
    {
        if (_forwardStack.TryPop(out var path))
        {
            _backwardStack.Push(path);
            await NavigateTo(path);
            CheckBackwardForwardCanExecute();
        }
    }

    public ReactiveCommand GoHome { get; }
    public ValueTask GoHomeAsync(CancellationToken cancel = default)
    {
        throw new NotImplementedException();
    }

    public NotifyCollectionChangedSynchronizedViewList<IPage> Pages { get; }

    public override IEnumerable<IRoutable> NavigationChildren
    {
        get
        {
            foreach (var page in _pages)
            {
                yield return page;
            }
        }
    }

    protected override ValueTask<IRoutable> NavigateToUnknownPath(ArraySegment<string> path)
    {
        var first = path[0];
        if (_container.TryGetExport<IPage>(first, out var page))
        {
            _pages.Add(page);
            page.NavigationParent = this;
            return page.NavigateTo(path[1..]);
        }

        return base.NavigateToUnknownPath(path);
    }

    public BindableReactiveProperty<ShellStatus> Status { get; }

    public ReactiveCommand Close { get; }

    protected override ValueTask InternalCatchEvent(AsyncRoutedEvent e)
    {
        if (e is ExecuteCommandEvent cmd)
        {
            // write command to log
        }

        if (e is NavigationEvent focus)
        {
            _backwardStack.Push([..focus.Source.GetAllTo(this).Select(x => x.Id)]);
            _forwardStack.Clear();
        }
        
        return ValueTask.CompletedTask;
    }
}

public enum ShellStatus
{
    Normal,
    Warning,
    Error,
}