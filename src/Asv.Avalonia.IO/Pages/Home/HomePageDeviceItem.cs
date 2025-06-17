using Asv.Common;
using Asv.IO;
using Material.Icons;
using Microsoft.Extensions.Logging;
using R3;

namespace Asv.Avalonia.IO;

public class HomePageDeviceItem : HomePageItem
{
    public HomePageDeviceItem(
        IClientDevice device,
        IDeviceManager deviceManager,
        ILoggerFactory loggerFactory
    )
        : base(NavigationId.NormalizeTypeId(device.Id.AsString()), loggerFactory)
    {
        Device = device;
        Icon = deviceManager.GetIcon(device.Id);
        IconBrush = deviceManager.GetDeviceBrush(device.Id);
        device.Name.Subscribe(x => Header = x).DisposeItWith(Disposable);
        Info.Add(
            new HeadlinedViewModel(DesignTime.Id, DesignTime.LoggerFactory)
            {
                Icon = MaterialIconKind.IdCard,
                Header = "StaticId",
                Description = device.Id.AsString(),
            }
        );
        Info.Add(
            new HeadlinedViewModel(DesignTime.Id, DesignTime.LoggerFactory)
            {
                Icon = MaterialIconKind.MergeType,
                Header = "Type",
                Description = device.Id.DeviceClass,
            }
        );
        var linkInfo = new HeadlinedViewModel(DesignTime.Id, DesignTime.LoggerFactory)
        {
            Icon = MaterialIconKind.Network,
            Header = "Link",
        };
        device
            .Link.State.Subscribe(x =>
            {
                linkInfo.Description = x.ToString("G");
            })
            .DisposeItWith(Disposable);
        Info.Add(linkInfo);
        Description = $"Device {device.Id.DeviceClass} with address {device.Id}";
    }

    public IClientDevice Device { get; }
}
