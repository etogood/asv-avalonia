using System.Collections.Immutable;
using System.Composition.Hosting;
using ObservableCollections;
using R3;

namespace Asv.Avalonia;

public abstract class Shell : RoutableViewModel, IShell
{
    private readonly CompositionHost _container;
    private readonly ObservableList<IShellPage> _pages = new();
    private readonly ImmutableDictionary<string, ICommandFactory> _commands;
    public const string ShellId = "shell";

    protected Shell(IContainerHost host)
        : base(ShellId)
    {
        ArgumentNullException.ThrowIfNull(host);
        
        _container = host.Host;
        Pages = _pages.ToNotifyCollectionChangedSlim();
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
            page.Parent = this;
            _pages.Add(page);
            return ValueTask.FromResult<IShellPage?>(page);
        }

        return ValueTask.FromResult<IShellPage?>(null);
    }

    protected abstract void InternalAddPageToMainTab(IShellPage export);

    public NotifyCollectionChangedSynchronizedViewList<IShellPage> Pages { get; }

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

    protected override ValueTask InternalCatchEvent(AsyncRoutedEvent e)
    {
        if (e is ExecuteCommandEvent cmd)
        {
            if (_commands.TryGetValue(cmd.CommandId, out var command))
            {
                 // write command to log
            }
        }

        if (e is FocusedEvent focus)
        {
            // write to navigation history
        }
        
        return ValueTask.CompletedTask;
    }
}