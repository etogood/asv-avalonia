using System.Composition.Hosting;

namespace Asv.Avalonia;

public interface IContainerHost
{
    bool TryGetExport<T>(string id, out T value);
}