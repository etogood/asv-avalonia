using System;
using System.Collections.Generic;
using System.Composition;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Asv.Common;
using DotNext.Collections.Generic;
using Microsoft.Extensions.Logging;
using ObservableCollections;
using R3;
using ZLogger;

namespace Asv.Avalonia.Example;

// TODO: add validation
public class SerialPortViewModel : DialogViewModelBase
{
    private readonly IMavlinkConnectionService? _service;
    private readonly ILogger _log;
    private int _requestNotComplete;
    private const int WriteBufferSizeConst = 40960;
    private const int WriteTimeoutConst = 1000;
    private const int DataBitsConst = 8;

    private readonly ObservableList<string> _myCache = [];

    [ImportingConstructor]
    public SerialPortViewModel(
        string id,
        IMavlinkConnectionService service,
        ILoggerFactory logFactory
    )
        : base(id)
    {
        _log = logFactory.CreateLogger<SerialPortViewModel>();
        _service = service;
        var currentIndex =
            service.Connections.Count(pair => pair.Value.TypeInfo.Scheme == "serial") + 1;
        Title = new BindableReactiveProperty<string>($"New Serial {currentIndex}");
        IsValid.Value = false;
        _sub1 = WriteBufferSizeInput.EnableValidation(
            value =>
            {
                if (!int.TryParse(value, out _))
                {
                    return ValueTask.FromResult<ValidationResult>(
                        new Exception($"{nameof(WriteBufferSizeInput)} is not int")
                    );
                }

                return ValidationResult.Success;
            },
            this,
            true
        );

        _sub2 = DataBitsInput.EnableValidation(
            value =>
            {
                if (!int.TryParse(value, out _))
                {
                    return ValueTask.FromResult<ValidationResult>(
                        new Exception($"{nameof(DataBitsInput)} is not int")
                    );
                }

                return ValidationResult.Success;
            },
            this,
            true
        );

        _sub3 = WriteTimeOutInput.EnableValidation(
            value =>
            {
                if (!int.TryParse(value, out _))
                {
                    return ValueTask.FromResult<ValidationResult>(
                        new Exception($"{nameof(WriteTimeOutInput)} is not int")
                    );
                }

                return ValidationResult.Success;
            },
            this,
            true
        );

        _sub4 = SelectedBoundRateInput.EnableValidation(
            value =>
            {
                if (!int.TryParse(value, out _))
                {
                    return ValueTask.FromResult<ValidationResult>(
                        new Exception($"{nameof(SelectedBoundRateInput)} is not int")
                    );
                }

                return ValidationResult.Success;
            },
            this,
            true
        );

        _sub5 = Title.EnableValidation(
            value =>
            {
                if (string.IsNullOrEmpty(value))
                {
                    return ValueTask.FromResult<ValidationResult>(
                        new Exception($"{nameof(Title)} is empty or null")
                    );
                }

                return ValidationResult.Success;
            },
            this,
            true
        );

        _sub6 = SelectedPortInput.EnableValidation(
            value =>
            {
                if (string.IsNullOrEmpty(value))
                {
                    return ValueTask.FromResult<ValidationResult>(
                        new Exception($"{nameof(SelectedPortInput)} is empty or null")
                    );
                }

                return ValidationResult.Success;
            },
            this,
            true
        );

        _sub7 = ParityInput.EnableValidation(
            value =>
            {
                if (value is null)
                {
                    return ValueTask.FromResult<ValidationResult>(
                        new Exception($"{nameof(ParityInput)} is null")
                    );
                }

                return ValidationResult.Success;
            },
            this,
            true
        );

        _sub8 = StopBitsInput.EnableValidation(
            value =>
            {
                if (value is null)
                {
                    return ValueTask.FromResult<ValidationResult>(
                        new Exception($"{nameof(StopBitsInput)} is null")
                    );
                }

                return ValidationResult.Success;
            },
            this,
            true
        );

        _sub9 = WriteBufferSizeInput.Subscribe(v => WriteBufferSize.Value = int.Parse(v));
        _sub10 = DataBitsInput.Subscribe(v => DataBits.Value = int.Parse(v));
        _sub11 = WriteTimeOutInput.Subscribe(v => WriteTimeOut.Value = int.Parse(v));

        Ports = _myCache.ToNotifyCollectionChanged();
        Observable
            .Timer(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(1))
            .Subscribe(_ => UpdateSerialPorts())
            .DisposeItWith(Disposable);
    }

    public override IEnumerable<IRoutable> GetRoutableChildren()
    {
        return [];
    }

    public void AddSerialPort()
    {
        if (_service == null)
        {
            return;
        }

        try
        {
            var connectionString =
                $"serial:{SelectedPortInput.CurrentValue}"
                + $"?br={SelectedBoundRateInput.CurrentValue}"
                + $"&wrt={WriteTimeOut.CurrentValue}"
                + $"&parity={ParityInput.CurrentValue}"
                + $"&dataBits={DataBits.CurrentValue}"
                + $"&stopBits={StopBitsInput.CurrentValue}"
                + $"&ws={WriteBufferSize.CurrentValue}";
            _service.AddConnection(Title.CurrentValue, connectionString);
        }
        catch (Exception? e)
        {
            _log.ZLogError($"Error to create port:{e.Message}", e);
        }
        finally
        {
            UpdateSerialPorts();
        }
    }

    private void UpdateSerialPorts()
    {
        if (Interlocked.CompareExchange(ref _requestNotComplete, 1, 0) != 0)
        {
            return;
        }

        try
        {
            var value = SerialPort.GetPortNames();
            var exist = _myCache.ToArray();
            var toDelete = exist.Except(value).ToArray();
            var toAdd = value.Except(exist).ToArray();
            foreach (var item in toDelete)
            {
                _myCache.Remove(item);
            }

            _myCache.AddAll(toAdd);
        }
        catch (Exception e)
        {
            _log.ZLogError($"Error to create port:{e.Message}", e);
        }
        finally
        {
            Interlocked.Exchange(ref _requestNotComplete, 0);
        }
    }

    public NotifyCollectionChangedSynchronizedViewList<string> Ports { get; set; }

    public BindableReactiveProperty<Array> BoundRates { get; } =
        new(new[] { 9600, 14400, 19200, 38400, 56000, 57600, 115200, 128000, 256000 });

    public BindableReactiveProperty<string> Title { get; set; }
    public BindableReactiveProperty<string> SelectedBoundRateInput { get; set; } = new();
    public BindableReactiveProperty<string> SelectedPortInput { get; set; } = new();
    public BindableReactiveProperty<Array> ParityValues => new(Enum.GetValues<Parity>());
    public BindableReactiveProperty<Parity?> ParityInput { get; set; } = new();
    public BindableReactiveProperty<string> WriteTimeOutInput { get; set; } =
        new(WriteTimeoutConst.ToString());
    private ReactiveProperty<int> WriteTimeOut { get; set; } = new(WriteTimeoutConst);
    public BindableReactiveProperty<string> WriteBufferSizeInput { get; set; } =
        new(WriteBufferSizeConst.ToString());
    private ReactiveProperty<int> WriteBufferSize { get; set; } = new(WriteBufferSizeConst);
    public BindableReactiveProperty<string> DataBitsInput { get; set; } =
        new(DataBitsConst.ToString());
    private ReactiveProperty<int> DataBits { get; set; } = new(DataBitsConst);
    public BindableReactiveProperty<StopBits?> StopBitsInput { get; set; } = new();

    public BindableReactiveProperty<Array> StopBitsArr => new(Enum.GetValues<StopBits>());

    #region Dispose

    private readonly IDisposable _sub1;
    private readonly IDisposable _sub2;
    private readonly IDisposable _sub3;
    private readonly IDisposable _sub4;
    private readonly IDisposable _sub5;
    private readonly IDisposable _sub6;
    private readonly IDisposable _sub7;
    private readonly IDisposable _sub8;
    private readonly IDisposable _sub9;
    private readonly IDisposable _sub10;
    private readonly IDisposable _sub11;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _sub1.Dispose();
            _sub2.Dispose();
            _sub3.Dispose();
            _sub4.Dispose();
            _sub5.Dispose();
            _sub6.Dispose();
            _sub7.Dispose();
            _sub8.Dispose();
            _sub9.Dispose();
            _sub10.Dispose();
            _sub11.Dispose();
        }

        base.Dispose(disposing);
    }

    #endregion
}
