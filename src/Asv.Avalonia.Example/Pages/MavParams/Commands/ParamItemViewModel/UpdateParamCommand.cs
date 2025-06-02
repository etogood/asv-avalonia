using System.Threading;
using System.Threading.Tasks;
using Asv.Common;
using Material.Icons;

namespace Asv.Avalonia.Example;

[ExportCommand]
public class UpdateParamCommand : ContextCommand<ParamItemViewModel>
{
    public const string Id = $"{BaseId}.params.item.update";

    internal static readonly ICommandInfo StaticInfo = new CommandInfo
    {
        Id = Id,
        Name = RS.UpdateParamCommand_CommandInfo_Name,
        Description = RS.UpdateParamCommand_CommandInfo_Description,
        Icon = MaterialIconKind.Update,
        DefaultHotKey = null,
        Source = SystemModule.Instance,
    };

    public override ICommandInfo Info => StaticInfo;

    protected override async ValueTask<CommandArg?> InternalExecute(
        ParamItemViewModel context,
        CommandArg newValue,
        CancellationToken cancel
    )
    {
        await context.UpdateImpl(cancel);
        return null;
    }
}
