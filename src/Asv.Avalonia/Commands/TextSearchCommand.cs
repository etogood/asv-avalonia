using System.Composition;
using Material.Icons;

namespace Asv.Avalonia;

[ExportCommand]
[Shared]
public class TextSearchCommand : ContextCommand<SearchBoxViewModel, StringArg>
{
    public const string Id = $"{BaseId}.search";

    public override ICommandInfo Info => new CommandInfo
    {
        Id = Id,
        Name = "Search",
        Description = "Search for text in the current context",
        Icon = MaterialIconKind.Search,
        DefaultHotKey = null,
        Source = SystemModule.Instance,
    };

    public override ValueTask<StringArg?> InternalExecute(SearchBoxViewModel context, StringArg arg, CancellationToken cancel)
    {
        var oldValue = context.PreviousTextSearch;
        context.Search.Execute(arg.Value);
        return ValueTask.FromResult<StringArg?>(CommandArg.FromString(oldValue));
    }

    public static ValueTask ExecuteCommand(SearchBoxViewModel owner, string? value)
    {
        return owner.ExecuteCommand(Id, CommandArg.FromString(value ?? string.Empty));
    }
}