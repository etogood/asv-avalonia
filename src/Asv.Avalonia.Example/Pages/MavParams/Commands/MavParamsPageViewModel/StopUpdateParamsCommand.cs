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
        Name = RS.StopUpdateParamsCommand_CommandInfo_Name,
        Description = RS.StopUpdateParamsCommand_CommandInfo_Description,
        Icon = MaterialIconKind.CancelCircle,
        DefaultHotKey = null,
        Source = SystemModule.Instance,
    };

    public override ICommandInfo Info => StaticInfo;

    protected override ValueTask<CommandArg?> InternalExecute(
        MavParamsPageViewModel context,
        CommandArg newValue,
        CancellationToken cancel
    )
    {
        context.StopUpdateParamsImpl();
        return default;
    }
}
