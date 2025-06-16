using System.Collections.Specialized;
using System.Diagnostics;
using Asv.Common;
using Asv.IO;
using Microsoft.Extensions.Logging;
using ObservableCollections;
using R3;
using ZLogger;

namespace Asv.Avalonia.IO;

public abstract class DevicePageViewModel<T> : PageViewModel<T>, IDevicePage
    where T : class, IDevicePage
{
    public const string ArgsDeviceIdKey = "dev_id";
    private readonly IDeviceManager? _devices;
    private string? _targetDeviceId;
    private IDisposable? _waitInitSubscription;
    private CancellationTokenSource? _deviceDisconnectedToken;
    private readonly ReactiveProperty<DeviceWrapper?> _target;
    private readonly ILogger<DevicePageViewModel<T>> _logger;

    protected DevicePageViewModel(
        NavigationId id,
        IDeviceManager devices,
        ICommandService cmd,
        ILoggerFactory logger
    )
        : base(id, cmd)
    {
        _devices = devices;
        _logger = logger.CreateLogger<DevicePageViewModel<T>>();
        _target = new ReactiveProperty<DeviceWrapper?>().DisposeItWith(Disposable);
        Disposable.AddAction(DeviceRemoved);
    }

    protected override void InternalInitArgs(NameValueCollection args)
    {
        _logger.ZLogTrace($"{this} init args: {args}");
        base.InternalInitArgs(args);
        Debug.Assert(_devices != null, "_devices != null");
        _targetDeviceId =
            args[ArgsDeviceIdKey]
            ?? throw new ArgumentNullException(
                $"{ArgsDeviceIdKey} argument is required for {GetType().Name} page"
            );
        _devices
            .Explorer.Devices.ObserveAdd()
            .Where(_targetDeviceId, (e, id) => e.Value.Key.AsString() == id)
            .Subscribe(x => DeviceFoundButNotInitialized(x.Value.Value))
            .DisposeItWith(Disposable);

        _devices
            .Explorer.Devices.ObserveRemove()
            .Where(_targetDeviceId, (e, id) => e.Value.Key.AsString() == id)
            .Subscribe(_ => DeviceRemoved())
            .DisposeItWith(Disposable);

        foreach (
            var device in _devices.Explorer.Devices.Where(x => x.Key.AsString() == _targetDeviceId)
        )
        {
            DeviceFoundButNotInitialized(device.Value);
        }
    }

    private void DeviceRemoved()
    {
        _deviceDisconnectedToken?.Cancel(false);
        _deviceDisconnectedToken?.Dispose();
        _deviceDisconnectedToken = null;
        _waitInitSubscription?.Dispose();
        _waitInitSubscription = null;
    }

    private void DeviceFoundButNotInitialized(IClientDevice device)
    {
        DeviceRemoved();
        _waitInitSubscription = device
            .State.Where(x => x == ClientDeviceState.Complete)
            .Take(1)
            .Subscribe(device, DeviceFoundAndInitialized);
    }

    private void DeviceFoundAndInitialized(ClientDeviceState state, IClientDevice device)
    {
        Debug.Assert(_waitInitSubscription != null, nameof(_waitInitSubscription) + " != null");
        _waitInitSubscription.Dispose();
        _deviceDisconnectedToken = new CancellationTokenSource();
        AfterDeviceInitialized(device, _deviceDisconnectedToken.Token);
        _target.OnNext(new DeviceWrapper(device, _deviceDisconnectedToken.Token));
    }

    protected abstract void AfterDeviceInitialized(
        IClientDevice device,
        CancellationToken onDisconnectedToken
    );
    public ReadOnlyReactiveProperty<DeviceWrapper?> Target => _target;
}
