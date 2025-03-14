using System;
using System.Collections.Generic;
using System.Composition;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Asv.Common;
using Asv.IO;
using Asv.Mavlink;
using ObservableCollections;
using R3;

namespace Asv.Avalonia.Example;

[ExportExtensionFor<IHomePage>]
public class HomePageDeviceListExtension : IExtensionFor<IHomePage>
{
    private readonly IMavlinkConnectionService _svc;
    private readonly IContainerHost _container;

    [ImportingConstructor]
    public HomePageDeviceListExtension(IMavlinkConnectionService svc, IContainerHost container)
    {
        ArgumentNullException.ThrowIfNull(svc);
        ArgumentNullException.ThrowIfNull(container);
        _svc = svc;
        _container = container;
    }

    public void Extend(IHomePage context, CompositeDisposable contextDispose)
    {
        _svc.DevicesExplorer.Devices.PopulateTo(context.Items, TryAdd, Remove)
            .DisposeItWith(contextDispose);
    }

    private bool Remove(KeyValuePair<DeviceId, IClientDevice> model, HomePageDevice vm)
    {
        return model.Key == vm.Device.Id;
    }

    private HomePageDevice TryAdd(KeyValuePair<DeviceId, IClientDevice> arg)
    {
        return new HomePageDevice(arg.Value);
    }
}
