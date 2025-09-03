using System.Collections.Immutable;

namespace Asv.Avalonia.FileAssociation;

public class NullFileAssociationService : IFileAssociationService
{
    public bool HasAnyHandlersForOpenFile { get; } = true;
    public ImmutableHashSet<string> OpenFileTypeFilters => ImmutableHashSet<string>.Empty;

    public void OpenFile(string path)
    {
        // do nothing
    }
}
