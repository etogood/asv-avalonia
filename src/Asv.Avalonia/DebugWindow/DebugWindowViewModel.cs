using System.Composition;
using ObservableCollections;
using R3;

namespace Asv.Avalonia;

[Export(typeof(IDebugWindow))]
public class DebugWindowViewModel : ViewModelBase, IDebugWindow
{
    public DebugWindowViewModel()
        : this(DesignTime.Shell)
    {
    }

    [ImportingConstructor]
    public DebugWindowViewModel(IShellHost host)
        : base(ModelId)
    {
        SelectedControlPath = host.Shell.SelectedControlPath.ToReadOnlyBindableReactiveProperty();
        Pages = host.Shell.Pages;
    }

    public NotifyCollectionChangedSynchronizedViewList<IPage> Pages { get; }
    public IReadOnlyBindableReactiveProperty<string[]?> SelectedControlPath { get; }

    public const string ModelId = "DebugWindow";
    protected override void Dispose(bool disposing)
    {
        // TODO: Implement Dispose
    }
}