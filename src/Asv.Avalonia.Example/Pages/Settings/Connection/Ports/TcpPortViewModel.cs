using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using R3;
using Exception = System.Exception;

namespace Asv.Avalonia.Example;

// TODO: add validation
public partial class TcpPortViewModel : DialogViewModelBase
{
    private ILogger _log;
    private readonly IMavlinkConnectionService _connectionService;
    private static readonly Regex IpRegex = IpRegexCtor();

    [ImportingConstructor]
    public TcpPortViewModel(
        string id,
        IMavlinkConnectionService connectionService,
        ILoggerFactory logFactory
    )
        : base(id)
    {
        _connectionService = connectionService;
        IsValid.Value = false;
        CreationNumber =
            connectionService.Connections.Count(_ => _.Value.TypeInfo.Scheme == "tcp") + 1;
        Title = new BindableReactiveProperty<string>($"New TCP {CreationNumber}");
        _log = logFactory.CreateLogger<TcpPortViewModel>();

        _sub1 = PortInput.EnableValidation(
            value =>
            {
                if (!(int.TryParse(value, out var result) && result is <= ushort.MaxValue and >= 0))
                {
                    return ValueTask.FromResult<ValidationResult>(
                        new Exception("Invalid port number")
                    );
                }

                return ValidationResult.Success;
            },
            this,
            true
        );

        _sub2 = IpAddressInput.EnableValidation(
            value =>
            {
                if (!IpRegex.IsMatch(value))
                {
                    return ValueTask.FromResult<ValidationResult>(
                        new Exception("Invalid ip address")
                    );
                }

                return ValidationResult.Success;
            },
            this,
            true
        );
    }

    public void AddTcpPort()
    {
        if (IsValid.CurrentValue)
        {
            var connectionString =
                $"tcp://{IpAddressInput.CurrentValue}:{PortInput.CurrentValue}"
                + (IsTcpIpServer.CurrentValue ? "?srv=true" : string.Empty);
            _connectionService.AddConnection(Title.CurrentValue, connectionString);
        }
    }

    private int CreationNumber { get; set; }
    public BindableReactiveProperty<string> Title { get; set; }
    public BindableReactiveProperty<string> IpAddressInput { get; set; } = new(string.Empty);
    public BindableReactiveProperty<string> PortInput { get; set; } = new(string.Empty);
    public BindableReactiveProperty<bool> IsTcpIpServer { get; set; } = new(false);

    public override IEnumerable<IRoutable> GetRoutableChildren()
    {
        return [];
    }

    [GeneratedRegex(@"^(\d{0,3}\.?){0,4}$")]
    private static partial Regex IpRegexCtor();

    #region Dispose

    private readonly IDisposable _sub1;
    private readonly IDisposable _sub2;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _sub1.Dispose();
            _sub2.Dispose();
        }

        base.Dispose(disposing);
    }

    #endregion
}
