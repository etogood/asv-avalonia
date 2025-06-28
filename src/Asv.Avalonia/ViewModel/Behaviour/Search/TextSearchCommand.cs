using System.Composition;
using Material.Icons;

namespace Asv.Avalonia;

[ExportCommand]
[Shared]
public class TextSearchCommand : ContextCommand<ISupportTextSearch, StringArg>
{
    public const string Id = $"{BaseId}.search.text";

    public override ICommandInfo Info =>
        new CommandInfo
        {
            Id = Id,
            Name = "Search text",
            Description = "Search for text in the current context",
            Icon = MaterialIconKind.Search,
            DefaultHotKey = null,
            Source = SystemModule.Instance,
        };

    public override ValueTask<StringArg?> InternalExecute(
        ISupportTextSearch context,
        StringArg arg,
        CancellationToken cancel
    )
    {
        var oldValue = context.SearchText;
        context.Query(arg.Value);
        return ValueTask.FromResult<StringArg?>(CommandArg.CreateString(oldValue));
    }

    public static ValueTask Execute(SearchBoxViewModel owner, string? value)
    {
        return owner.ExecuteCommand(Id, CommandArg.CreateString(value ?? string.Empty));
    }
}
