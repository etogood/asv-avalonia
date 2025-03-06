using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Asv.Common;
using Asv.IO;
using Material.Icons;
using ObservableCollections;
using R3;

namespace Asv.Avalonia.Example;

[ExportExtensionFor<IHomePage>]
[method: ImportingConstructor]
public class HomePageDeviceExtension(IMavlinkConnectionService svc)
    : AsyncDisposableOnce,
        IExtensionFor<IHomePage>
{
    private IDisposable? _sub1;
    private IDisposable? _sub2;

    public void Extend(IHomePage context)
    {
        _sub1 = svc
            .DevicesExplorer.Devices.ObserveAdd()
            .Subscribe(x => Add(x.Value.Value, context));
        _sub2 = svc.DevicesExplorer.Devices.ObserveRemove().Subscribe(x => Remove(x, context));
        foreach (var device in svc.DevicesExplorer.Devices.Values)
        {
            Add(device, context);
        }
    }

    private void Add(IClientDevice device, IHomePage context)
    {
        context.Items.Add(new ClientDeviceAdapter(device));
    }

    private static void Remove(
        CollectionRemoveEvent<KeyValuePair<DeviceId, IClientDevice>> arg,
        IHomePage context
    )
    {
        var itemToDelete = context.Items.FirstOrDefault(x =>
            x is ClientDeviceAdapter adapter && adapter.Device.Id == arg.Value.Key
        );
        if (itemToDelete != null)
        {
            context.Items.Remove(itemToDelete);
            itemToDelete.Dispose();
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _sub1?.Dispose();
            _sub2?.Dispose();
        }

        base.Dispose(disposing);
    }

    protected override async ValueTask DisposeAsyncCore()
    {
        if (_sub1 != null)
        {
            await CastAndDispose(_sub1);
        }

        if (_sub2 != null)
        {
            await CastAndDispose(_sub2);
        }

        await base.DisposeAsyncCore();

        return;

        static async ValueTask CastAndDispose(IDisposable resource)
        {
            if (resource is IAsyncDisposable resourceAsyncDisposable)
            {
                await resourceAsyncDisposable.DisposeAsync();
            }
            else
            {
                resource.Dispose();
            }
        }
    }
}

public class ClientDeviceAdapter : HomePageItem
{
    public ClientDeviceAdapter(IClientDevice device)
        : base(device.Id.AsString())
    {
        Device = device;
        Icon = DeviceIconMixin.GetIcon(device.Id);
        device.Name.Subscribe(x => Header = x).DisposeItWith(Disposable);

        Info.Add(
            new HeadlinedViewModel("id")
            {
                Icon = MaterialIconKind.IdCard,
                Header = "Id",
                Description = device.Id.AsString(),
            }
        );
        Info.Add(
            new HeadlinedViewModel("type")
            {
                Icon = MaterialIconKind.MergeType,
                Header = "Type",
                Description = device.Id.DeviceClass,
            }
        );
        var linkInfo = new HeadlinedViewModel("link")
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
