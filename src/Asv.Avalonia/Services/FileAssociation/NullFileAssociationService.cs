using System.Collections.Immutable;

namespace Asv.Avalonia.FileAssociation;

public class NullFileAssociationService : IFileAssociationService
{
    public IEnumerable<FileTypeInfo> SupportedFiles { get; } = [];

    public ValueTask Open(string path)
    {
        // do nothing
        return ValueTask.CompletedTask;
    }

    public ValueTask Create(string path, FileTypeInfo type)
    {
        return ValueTask.CompletedTask;
    }
}
