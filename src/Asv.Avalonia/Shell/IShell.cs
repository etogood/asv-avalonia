using System.Composition;
using System.Composition.Hosting;
using ObservableCollections;
using R3;

namespace Asv.Avalonia;

public interface IShellHost
{
    IShell Shell { get; }
}

public interface IShell : IRoutableViewModel
{
    ReactiveCommand Back { get; }
    ValueTask BackwardAsync(CancellationToken cancel = default);
    ReactiveCommand Forward { get; }
    ValueTask ForwardAsync(CancellationToken cancel = default);
    ReactiveCommand GoHome { get; }
    ValueTask GoHomeAsync(CancellationToken cancel = default);
    ValueTask<IShellPage?> OpenPage(string pageId);
    IReadOnlyObservableList<IShellPage> Pages { get; }
}

public abstract class Shell : RoutableViewModel, IShell
{
    private readonly CompositionHost _container;
    private readonly ObservableList<IShellPage> _pages = new();
    public const string ShellId = "shell";

    protected Shell(IContainerHost host)
        : base(ShellId)
    {
        ArgumentNullException.ThrowIfNull(host);
        _container = host.Host;
        Back = new ReactiveCommand((_, c) => BackwardAsync(c));
        Forward = new ReactiveCommand((_, c) => ForwardAsync(c));
        GoHome = new ReactiveCommand((_, c) => BackwardAsync(c));
    }

    public ReactiveCommand Back { get; }
    public ValueTask BackwardAsync(CancellationToken cancel = default)
    {
        throw new NotImplementedException();
    }

    public ReactiveCommand Forward { get; }
    public ValueTask ForwardAsync(CancellationToken cancel = default)
    {
        throw new NotImplementedException();
    }

    public ReactiveCommand GoHome { get; }
    public ValueTask GoHomeAsync(CancellationToken cancel = default)
    {
        throw new NotImplementedException();
    }

    public ValueTask<IShellPage?> OpenPage(string pageId)
    {
        if (_container.TryGetExport<IShellPage>(pageId, out var page))
        {
            InternalAddPageToMainTab(page);
            return ValueTask.FromResult<IShellPage?>(page);
        }

        return ValueTask.FromResult<IShellPage?>(null);
    }

    protected abstract void InternalAddPageToMainTab(IShellPage export);

    public IReadOnlyObservableList<IShellPage> Pages => _pages;

    public override IEnumerable<IRoutableViewModel> Children
    {
        get
        {
            foreach (var page in _pages)
            {
                yield return page;
            }
        }
    }

    protected override ValueTask InternalCatchEvent(AsyncRoutedEvent asyncRoutedEvent)
    {
        return ValueTask.CompletedTask;
    }
    
}