using Asv.Common;
using Asv.IO;

namespace Asv.Avalonia;

public class CommandHistoryFile(string path, int capacity)
    : RingFile<CommandSnapshot, CommandHistoryFileMetadata>(
        path,
        capacity,
        FileFormat,
        new CommandHistoryFileMetadata()
    )
{
    public static RingFileFormat FileFormat =>
        new(
            "Asv.Drones.CommandHistory",
            SemVersion.Parse("1.0.0"),
            1024 // 1024 bytes for metadata
        );
}

public class CommandHistoryFileMetadata : ISizedSpanSerializable
{
    public void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        // do nothing, metadata is empty
    }

    public void Serialize(ref Span<byte> buffer)
    {
        // do nothing, metadata is empty
    }

    public int GetByteSize()
    {
        return 0;
    }
}
