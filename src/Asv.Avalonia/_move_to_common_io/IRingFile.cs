using Asv.IO;

namespace Asv.Avalonia;

public interface IRingFile<TData, TMetadata> : IDisposable
    where TData : ISizedSpanSerializable
    where TMetadata : ISizedSpanSerializable
{
    RingFileFormat Format { get; }
    TMetadata ReadMetadata();
    void WriteMetadata(TMetadata metadata);
    void EditMetadata(Action<TMetadata> editAction);

    void Push(TData data);
    TData Pop();
}
