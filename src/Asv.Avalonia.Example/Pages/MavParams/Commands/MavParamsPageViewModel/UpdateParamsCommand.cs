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
        Name = "Refresh params",
        Description = "Command that refreshes params from the drone",
        Icon = MaterialIconKind.Refresh,
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
        context.UpdateParamsImpl();
        return default;
    }
}
