using System.Collections.Immutable;
using System.Composition;
using Microsoft.Extensions.Hosting;

namespace Asv.Avalonia.FileAssociation;

[Export(typeof(IFileAssociationService))]
[Shared]
public class FileAssociationService : IFileAssociationService
{
    private readonly ImmutableArray<IFileHandler> _handlers;

    [ImportingConstructor]
    public FileAssociationService([ImportMany] IEnumerable<IFileHandler> handlers)
    {
        _handlers = [.. handlers.OrderBy(x => x.Priority)];
        OpenFileTypeFilters = _handlers.SelectMany(x => x.OpenFileTypeFilters).ToImmutableHashSet();
    }

    public bool HasAnyHandlersForOpenFile => _handlers.Any();
    public ImmutableHashSet<string> OpenFileTypeFilters { get; }

    public void OpenFile(string path)
    {
        foreach (var handler in _handlers)
        {
            if (handler.CanOpen(path))
            {
                handler.Open(path);
                return;
            }
        }
        throw new NotSupportedException($"No handler found for file {path}");
    }
}
