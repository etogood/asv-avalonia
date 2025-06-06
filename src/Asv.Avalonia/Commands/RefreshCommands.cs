using System.Composition;
using Asv.Avalonia;
using Material.Icons;

[ExportCommand]
[Shared]
public class RefreshCommand : ContextCommand<ISupportRefresh>
{
    public const string Id = $"{BaseId}.refresh";

    public override ICommandInfo Info =>
        new CommandInfo
        {
            Id = Id,
            Name = "Refresh",
            Description = "Refresh the current page",
            Icon = MaterialIconKind.Refresh,
            DefaultHotKey = "Ctrl+F5",
            Source = SystemModule.Instance,
        };

    protected override ValueTask<CommandArg?> InternalExecute(
        ISupportRefresh context,
        CommandArg newValue,
        CancellationToken cancel
    )
    {
        context.Refresh();
        return ValueTask.FromResult<CommandArg?>(null);
    }
}
