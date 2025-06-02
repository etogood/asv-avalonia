using System.Buffers;
using Newtonsoft.Json;

namespace Asv.Avalonia;

public partial class CommandArg
{
    public static CommandArg Null => EmptyArg.Instance;
}

public class EmptyArg : CommandArg
{
    public static CommandArg Instance { get; } = new EmptyArg();

    public static CommandArg Create() => Instance;

    public override Id TypeId => Id.Empty;

    protected override void InternalDeserialize(ref ReadOnlySpan<byte> buffer)
    {
        // No data to serialize
    }

    protected override void InternalSerialize(ref Span<byte> buffer)
    {
        // No data to serialize
    }

    protected override int InternalGetByteSize() => 0;

    public override string ToString() => "[EMPTY]";
}
