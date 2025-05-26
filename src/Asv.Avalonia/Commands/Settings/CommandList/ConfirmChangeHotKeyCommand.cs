using System.Composition;
using Material.Icons;

namespace Asv.Avalonia;

[ExportCommand]
[Shared]
public sealed class ConfirmChangeHotKeyCommand : ContextCommand<SettingsCommandListItemViewModel>
{
    #region Static

    public const string Id = $"{BaseId}.settings.commandlist.item.hotkey.confirm";

    private static readonly ICommandInfo StaticInfo = new CommandInfo
    {
        Id = Id,
        Name = RS.ConfirmChangeHotKeyCommand_CommandInfo_Name,
        Description = RS.ConfirmChangeHotKeyCommand_CommandInfo_Description,
        Icon = MaterialIconKind.KeyboardCaps,
        HotKeyInfo = new HotKeyInfo { DefaultHotKey = null },
        Source = SystemModule.Instance,
    };

    #endregion

    public override ICommandInfo Info => StaticInfo;

    protected override ValueTask<ICommandArg?> InternalExecute(
        SettingsCommandListItemViewModel context,
        ICommandArg newValue,
        CancellationToken cancel
    )
    {
        if (newValue is not StringCommandArg arg)
        {
            throw new CommandArgMismatchException(typeof(StringCommandArg));
        }

        var oldValue = context.Info.HotKeyInfo.CustomHotKey.Value?.ToString() ?? string.Empty;
        context.CurrentHotKeyString.Value = arg.Value;
        context.ConfirmChangeHotKeyImpl();

        return ValueTask.FromResult<ICommandArg?>(new StringCommandArg(oldValue));
    }
}
