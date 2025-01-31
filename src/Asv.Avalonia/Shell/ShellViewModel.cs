using System.Collections.Immutable;
using System.Composition;
using System.Composition.Hosting;
using Avalonia;
using ObservableCollections;
using R3;

namespace Asv.Avalonia;

public class ShellViewModel : RoutableViewModel, IShell
{
    
    private readonly ObservableList<IPage> _pages = new();
    private readonly IContainerHost _container;
    private readonly Stack<string[]> _backwardStack = new();
    private readonly Stack<string[]> _forwardStack = new();
    private readonly IDisposable _sub1;
    private bool _internalNavigation;
    private bool _internalChange;
    private readonly BindableReactiveProperty<IRoutable> _selectedControl;
    private readonly BindableReactiveProperty<string[]?> _selectedControlPath;
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
        _sub1 = SelectedPage.SubscribeAwait(async (x, _) =>
        {
            if (x == null || _internalChange)
            {
                return;
            }

            var page = await NavigateTo(x.Id);
            await Rise(new NavigationEvent(page));
        });
        _selectedControl = new BindableReactiveProperty<IRoutable>(this);
        _selectedControlPath = new BindableReactiveProperty<string[]?>();
        _selectedControlPath.Subscribe(x =>
        {
            if (x is not { Length: > 0 })
            {
                return;
            }

            _backwardStack.Push(x);
            _forwardStack.Clear();
        });
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
            _internalNavigation = true;
            await this.NavigateTo(path);
            _internalNavigation = false;
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
            _internalNavigation = true;
            await this.NavigateTo(path);
            _internalNavigation = false;
            CheckBackwardForwardCanExecute();
        }
    }

    public ReactiveCommand GoHome { get; }
    public async ValueTask GoHomeAsync(CancellationToken cancel = default)
    {
        await NavigateTo(HomePageViewModel.PageId);
    }

    public NotifyCollectionChangedSynchronizedViewList<IPage> Pages { get; }

    public IReadOnlyBindableReactiveProperty<IRoutable> SelectedControl => _selectedControl;
    public IReadOnlyBindableReactiveProperty<string[]?> SelectedControlPath => _selectedControlPath;

    public BindableReactiveProperty<ShellStatus> Status { get; }
    public ReactiveCommand Close { get; }
    public BindableReactiveProperty<IPage?> SelectedPage { get; } = new();

    public override ValueTask<IRoutable> NavigateTo(string id)
    {
        var page = _pages.FirstOrDefault(x => x.Id == id);
        if (page == null)
        {
            if (_container.TryGetExport<IPage>(id, out page))
            {
                _pages.Add(page);
                page.Parent = this;
            }
        }

        _internalChange = true;
        SelectedPage.Value = page;
        _internalChange = false;
        return ValueTask.FromResult<IRoutable>(page);
    }

    protected override ValueTask InternalCatchEvent(AsyncRoutedEvent e)
    {
        if (e is ExecuteCommandEvent cmd)
        {
            // write command to log
        }

        if (e is NavigationEvent focus && _internalNavigation == false)
        {
            _selectedControl.Value = focus.Source;
            _selectedControlPath.Value = focus.Source.GetAllFrom(this).Skip(1).Select(x => x.Id).ToArray();
            CheckBackwardForwardCanExecute();
        }

        return ValueTask.CompletedTask;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _sub1.Dispose();
            Back.Dispose();
            Forward.Dispose();
            GoHome.Dispose();
            Pages.Dispose();
            Status.Dispose();
            Close.Dispose();
            SelectedPage.Dispose();
        }

        base.Dispose(disposing);
    }
}

public enum ShellStatus
{
    Normal,
    Warning,
    Error,
}

