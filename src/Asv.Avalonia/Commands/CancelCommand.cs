using System.Composition;
using Material.Icons;

namespace Asv.Avalonia;

[ExportCommand]
[Shared]
public class CancelCommand : ContextCommand<ISupportCancel>
{
    public const string Id = $"{BaseId}.cancel";

    public override ICommandInfo Info =>
        new CommandInfo
        {
            Id = Id,
            Name = "Cancel",
            Description = "Cancel the current operation",
            Icon = MaterialIconKind.Cancel,
            DefaultHotKey = "Shift+F5",
            Source = SystemModule.Instance,
        };

    protected override ValueTask<CommandArg?> InternalExecute(
        ISupportCancel context,
        CommandArg newValue,
        CancellationToken cancel
    )
    {
        context.Cancel();
        return CommandArg.Null;
    }
}
