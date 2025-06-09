using System.Diagnostics;
using Asv.Common;
using Asv.IO;
using R3;

namespace Asv.Avalonia.IO;

public abstract class DevicePageViewModel<T> : PageViewModel<T>, IDevicePage
    where T : class, IDevicePage
{
    private readonly IDeviceManager? _devices;
    private readonly Subject<IClientDevice> _afterDeviceInitializedCallback;

    protected DevicePageViewModel(NavigationId id, IDeviceManager devices, ICommandService cmd)
        : base(id, cmd)
    {
        _devices = devices;
        _afterDeviceInitializedCallback = new Subject<IClientDevice>().DisposeItWith(Disposable);
    }

    protected override void InternalInitArgs(string? args)
    {
        Debug.Assert(_devices != null, "_devices != null");
        base.InternalInitArgs(args);

        Device = _devices.Explorer.Devices.Values.FirstOrDefault(x => x.Id.AsString() == args);
        if (Device == null)
        {
            // TODO: say user that device not found
            return;
        }

        // we need to wait until device is initialized
        Device
            .State.Where(x => x == ClientDeviceState.Complete)
            .Take(1)
            .Subscribe(_ => AfterDeviceInitialized(Device))
            .DisposeItWith(Disposable);
    }

    protected virtual void AfterDeviceInitialized(IClientDevice device)
    {
        _afterDeviceInitializedCallback.OnNext(device);
    }

    public IClientDevice? Device { get; private set; }

    public Observable<IClientDevice> OnDeviceInitialized => _afterDeviceInitializedCallback;
}
