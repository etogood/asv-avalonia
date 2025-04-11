using System.Threading;
using System.Threading.Tasks;
using Material.Icons;

namespace Asv.Avalonia.Example;

[ExportCommand]
public class StopUpdateParamsCommand : ContextCommand<MavParamsPageViewModel>
{
    public const string Id = $"{BaseId}.params.stop-update";

    internal static readonly ICommandInfo StaticInfo = new CommandInfo
    {
        Id = Id,
        Name = "Stop refreshing params",
        Description = "Command to stop receiving parameters from the drone",
        Icon = MaterialIconKind.CancelCircle,
        DefaultHotKey = null, // TODO: make a key bind when new key listener system appears
        Source = SystemModule.Instance,
    };

    public override ICommandInfo Info => StaticInfo;

    protected override ValueTask<ICommandArg?> InternalExecute(
        MavParamsPageViewModel context,
        ICommandArg newValue,
        CancellationToken cancel
    )
    {
        context.StopUpdateParamsImpl();
        return default;
    }
}
