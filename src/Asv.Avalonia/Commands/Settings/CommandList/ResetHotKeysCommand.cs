using System.Composition;
using Material.Icons;

namespace Asv.Avalonia;

[ExportCommand]
[Shared]
public class ResetHotKeysCommand : ContextCommand<SettingsCommandListViewModel>
{
    #region Static

    public const string Id = $"{BaseId}.settings.commandlist.hotkeys.reset";

    private static readonly ICommandInfo StaticInfo = new CommandInfo
    {
        Id = Id,
        Name = RS.ResetHotKeysCommand_CommandInfo_Name,
        Description = RS.ResetHotKeysCommand_CommandInfo_Description,
        Icon = MaterialIconKind.KeyboardCaps,
        HotKeyInfo = new HotKeyInfo { DefaultHotKey = null },
        Source = SystemModule.Instance,
    };

    #endregion

    public override ICommandInfo Info => StaticInfo;

    protected override ValueTask<ICommandArg?> InternalExecute(
        SettingsCommandListViewModel context,
        ICommandArg newValue,
        CancellationToken cancel
    )
    {
        if (newValue is not ListCommandArg<(NavigationId, string?)> arg)
        {
            return ValueTask.FromException<ICommandArg?>(
                new CommandArgMismatchException(typeof(ListCommandArg<(NavigationId, string?)>))
            );
        }

        if (arg.Items is null)
        {
            return ValueTask.FromException<ICommandArg?>(
                new InvalidOperationException("Unable to perform action. Pass a valid parameter.")
            );
        }

        if (arg.Items is not null && arg.Items?.Count != 0)
        {
            foreach (var item in context.Items)
            {
                var vm = arg
                    .Items?.Where(it =>
                        it.Item1 == item.Id && it.Item2 != item.CurrentHotKeyString.Value
                    )
                    .FirstOrDefault();

                if (vm is null)
                {
                    continue;
                }

                var key = vm.Value.Item2;

                item.CurrentHotKeyString.Value = key;
                item.ConfirmChangeHotKeyImpl();
            }

            return ValueTask.FromResult<ICommandArg?>(
                new ListCommandArg<(NavigationId, string?)>([])
            );
        }

        var oldItems = context.Items.Select(x => (x.Id, x.CurrentHotKeyString.Value)).ToList();
        foreach (var item in context.Items)
        {
            item.IsReset.Value = true;
        }

        return ValueTask.FromResult<ICommandArg?>(
            new ListCommandArg<(NavigationId, string?)>(oldItems)
        );
    }
}
