using Asv.IO;
using R3;

namespace Asv.Avalonia.IO;

public interface IDevicePage : IPage
{
    IClientDevice? Device { get; }

    Observable<IClientDevice> OnDeviceInitialized { get; }
}
