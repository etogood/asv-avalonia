using System.Composition;
using R3;

namespace Asv.Avalonia;

[ExportCommand]
[Shared]
public class PaginationCommand : ContextCommand<ISupportPagination, ListArg>
{
    public const string Id = $"{BaseId}.pagination";

    public static readonly ICommandInfo StaticInfo = new CommandInfo
    {
        Id = Id,
        Name = "Pagination",
        Description = "Change pagination parameters",
        Icon = Material.Icons.MaterialIconKind.ViewList,
        DefaultHotKey = null,
        Source = SystemModule.Instance,
    };

    public override ICommandInfo Info => StaticInfo;

    public override ValueTask<ListArg?> InternalExecute(
        ISupportPagination context,
        ListArg arg,
        CancellationToken cancel
    )
    {
        var old = new ListArg(2) { new IntArg(context.Skip.Value), new IntArg(context.Take.Value) };
        var skip = ((IntArg)arg[0]).Value;
        var take = ((IntArg)arg[1]).Value;
        context.Skip.Value = (int)skip;
        context.Take.Value = (int)take;
        return ValueTask.FromResult<ListArg?>(old);
    }

    public static ValueTask Execute(IRoutable context, int skip, int take)
    {
        return context.ExecuteCommand(Id, new ListArg(2) { new IntArg(skip), new IntArg(take) });
    }
}

public interface ISupportPagination : IRoutable
{
    BindableReactiveProperty<int> Skip { get; }
    BindableReactiveProperty<int> Take { get; }
}
