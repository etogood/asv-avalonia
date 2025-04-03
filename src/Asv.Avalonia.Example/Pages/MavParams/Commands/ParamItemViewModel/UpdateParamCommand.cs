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
        Name = "Update param",
        Description = "Command that updates param from the drone",
        Icon = MaterialIconKind.Update,
        DefaultHotKey = null, // TODO: make a key bind when new key listener system appears
        Source = SystemModule.Instance,
    };

    public override ICommandInfo Info => StaticInfo;

    protected override ValueTask<ICommandArg?> InternalExecute(
        ParamItemViewModel context,
        ICommandArg newValue,
        CancellationToken cancel
    )
    {
        context.UpdateImpl(cancel).SafeFireAndForget();
        return default;
    }
}
