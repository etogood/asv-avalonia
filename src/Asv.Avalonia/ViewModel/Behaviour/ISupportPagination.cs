using Asv.Avalonia.Routable;
using R3;

namespace Asv.Avalonia;

public interface ISupportPagination : IRoutable
{
    BindableReactiveProperty<int> Skip { get; }
    BindableReactiveProperty<int> Take { get; }
}
