using Asv.Avalonia.Routable;

namespace Asv.Avalonia;

public interface ISupportCancel : IRoutable
{
    void Cancel();
}
