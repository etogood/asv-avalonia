using R3;

namespace Asv.Avalonia;

public interface IShellPage : IRoutableViewModel
{
    public IReadOnlyBindableReactiveProperty<string> Title { get; }
}