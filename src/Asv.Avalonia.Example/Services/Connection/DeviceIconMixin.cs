using Asv.IO;
using Asv.Mavlink;
using Material.Icons;

namespace Asv.Avalonia.Example;

public static class DeviceIconMixin
{
    public static MaterialIconKind GetIcon(DeviceId deviceId)
    {
        switch (deviceId.DeviceClass)
        {
            case Vehicles.PlaneDeviceClass:
                return MaterialIconKind.Plane;
            case Vehicles.CopterDeviceClass:
                return MaterialIconKind.Navigation;
            case GbsClientDevice.DeviceClass:
                return MaterialIconKind.RouterWireless;
            default:
                return MaterialIconKind.Memory;
        }
    }
}
