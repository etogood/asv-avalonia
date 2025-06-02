using Asv.Common;
using Asv.IO;

namespace Asv.Avalonia;

public interface IRingFile<TData, out TMetadata> : IDisposable
    where TData : ISizedSpanSerializable
    where TMetadata : ISizedSpanSerializable
{
    RingFileFormat Format { get; }
    IRingFileInfo Info { get; }
    TMetadata Metadata { get; }
    void EditMetadata(Action<TMetadata> editAction);
}

public interface IRingFileInfo
{
    uint ItemsCount { get; }
    uint Size { get; }
}
