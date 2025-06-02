using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Material.Icons;
using R3;

namespace Asv.Avalonia.Example;

[ExportCommand]
public class RemoveAllPinsCommand : ContextCommand<MavParamsPageViewModel>
{
    public const string Id = $"{BaseId}.params.remove-all-pins";

    internal static readonly ICommandInfo StaticInfo = new CommandInfo
    {
        Id = Id,
        Name = RS.UnpinAllParamsCommand_CommandInfo_Name,
        Description = RS.UnpinAllParamsCommand_CommandInfo_Description,
        Icon = MaterialIconKind.PinOff,
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
        return ValueTask.FromResult<CommandArg?>(null);
        /*var value = newValue as ListArg;

        context.SelectedItem.Value = null;

        if (value?.Items is null)
        {
            var pinned = new List<ParamItemViewModel>();
            foreach (var item in context.AllParams.Where(item => item.IsPinned.ViewValue.Value))
            {
                pinned.Add(item);
                item.IsPinned.ViewValue.Value = false;
            }

            foreach (var item in pinned)
            {
                context.ViewedParams.Remove(item);
            }

            var oldValue = new ListArg(pinned);

            return ValueTask.FromResult<CommandArg?>(oldValue);
        }

        foreach (var item in context.AllParams)
        {
            var oldItem = value.Items.FirstOrDefault(it => it.Name == item.Name);

            if (oldItem is null)
            {
                continue;
            }

            item.PinItem.Execute(Unit.Default);

            if (item.IsPinned.ViewValue.Value)
            {
                context.ViewedParams.Add(item);
            }
        }

        var oldValue1 = new ListArg(value.Items);

        return ValueTask.FromResult<CommandArg?>(oldValue1);*/
    }
}
