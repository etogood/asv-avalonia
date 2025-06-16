using Asv.IO;
using R3;

namespace Asv.Avalonia.IO;

public readonly struct DeviceWrapper(IClientDevice device, CancellationToken whenDisconnectedToken)
{
    public IClientDevice Device { get; } = device;
    public CancellationToken WhenDisconnectedToken { get; } = whenDisconnectedToken;
}

public interface IDevicePage : IPage
{
    ReadOnlyReactiveProperty<DeviceWrapper?> Target { get; }
}
