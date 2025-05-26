using Asv.Avalonia.GeoMap;
using Asv.IO;

namespace Asv.Avalonia.Example;

public interface IUavFlightWidget : IMapWidget
{
    IClientDevice Device { get; }
}
