using System.Composition;
using Asv.Avalonia.Routable;
using Asv.Common;
using Material.Icons;

namespace Asv.Avalonia;

[ExportCommand]
[Shared]
public class FocusSearchBoxCommand : ContextCommand<IRoutable>
{
    public const string Id = $"{BaseId}.search.focus";

    public override ICommandInfo Info =>
        new CommandInfo
        {
            Id = Id,
            Name = "Go to search box",
            Description = "Focus on the search box to start searching",
            Icon = MaterialIconKind.Search,
            DefaultHotKey = "Ctrl+F",
            Source = SystemModule.Instance,
        };
    protected override async ValueTask<CommandArg?> InternalExecute(IRoutable context, CommandArg newValue, CancellationToken cancel)
    {
        var found = await TreeVisitorEvent.VisitAll<ISupportTextSearch>(context);
        if (found.Count == 0)
        {
            return null;
        }
        
        // we assume that the ISearchBox with the longest path to root is the main search box
        found.MaxItem(x => x.GetPathToRoot().Count).Focus();
        return null;
    }
}