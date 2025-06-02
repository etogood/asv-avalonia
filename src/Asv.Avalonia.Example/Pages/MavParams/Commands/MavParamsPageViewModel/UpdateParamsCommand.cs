using System.Threading;
using System.Threading.Tasks;
using Material.Icons;

namespace Asv.Avalonia.Example;

[ExportCommand]
public class UpdateParamsCommand : ContextCommand<MavParamsPageViewModel>
{
    public const string Id = $"{BaseId}.params.update";

    internal static readonly ICommandInfo StaticInfo = new CommandInfo
    {
        Id = Id,
        Name = RS.UpdateParamsCommand_CommandInfo_Name,
        Description = RS.UpdateParamsCommand_CommandInfo_Description,
        Icon = MaterialIconKind.Refresh,
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
        context.UpdateParamsImpl();
        return default;
    }
}
