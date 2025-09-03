using System.Collections.Immutable;

namespace Asv.Avalonia.FileAssociation;

public interface IFileHandler
{
    int Priority { get; }
    IEnumerable<string> OpenFileTypeFilters { get; }
    bool CanOpen(string path);
    void Open(string path);
}

public interface IFileAssociationService
{
    bool HasAnyHandlersForOpenFile { get; }
    ImmutableHashSet<string> OpenFileTypeFilters { get; }
    void OpenFile(string path);
}
