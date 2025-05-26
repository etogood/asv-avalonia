using System.Composition;
using Material.Icons;

namespace Asv.Avalonia;

[ExportCommand]
[Shared]
public class ChangeBoolPropertyCommand : ContextCommand<IHistoricalProperty<bool>>
{
    #region Static

    public const string Id = $"{BaseId}.property.bool";

    private static readonly ICommandInfo StaticInfo = new CommandInfo
    {
        Id = Id,
        Name = RS.ChangeBoolPropertyCommand_CommandInfo_Name,
        Description = RS.ChangeBoolPropertyCommand_CommandInfo_Description,
        Icon = MaterialIconKind.PropertyTag,
        HotKeyInfo = new HotKeyInfo { DefaultHotKey = null },
        Source = SystemModule.Instance,
    };

    public override ICommandInfo Info => StaticInfo;

    #endregion

    protected override ValueTask<ICommandArg?> InternalExecute(
        IHistoricalProperty<bool> context,
        ICommandArg newValue,
        CancellationToken cancel
    )
    {
        if (newValue is not BoolCommandArg value)
        {
            throw new CommandArgMismatchException(typeof(BoolCommandArg));
        }

        var oldValue = new BoolCommandArg(context.ModelValue.Value);
        context.ModelValue.OnNext(value.Value);
        return ValueTask.FromResult<ICommandArg?>(oldValue);
    }
}
