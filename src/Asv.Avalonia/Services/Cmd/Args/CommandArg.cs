using System.Diagnostics;
using System.Runtime.CompilerServices;
using Asv.IO;

namespace Asv.Avalonia;

public abstract partial class CommandArg : ISizedSpanSerializable
{
    public enum Id : uint
    {
        Empty = 0,
        Bool = 1,
        String = 2,
        Double = 3,
        Action = 4,
        Array = 5,
        Dict = 6,
    }

    public static CommandArg Create(Id id)
    {
        return id switch
        {
            Id.Empty => EmptyArg.Create(),
            Id.Bool => BoolArg.Create(),
            Id.String => StringArg.Create(),
            Id.Double => DoubleArg.Create(),
            Id.Action => ActionArg.Create(),
            Id.Array => ListArg.Create(),
            Id.Dict => DictArg.Create(),
            _ => throw new ArgumentOutOfRangeException(nameof(id), id, null),
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Id ReadTypeId(ref ReadOnlySpan<byte> buffer) =>
        (Id)BinSerialize.ReadPackedUnsignedInteger(ref buffer);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteTypeId(ref Span<byte> buffer, Id typeId) =>
        BinSerialize.WritePackedUnsignedInteger(ref buffer, (uint)typeId);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetSizeOfTypeId(Id typeId) =>
        BinSerialize.GetSizeForPackedUnsignedInteger((uint)typeId);

    public static CommandArg Create(ref ReadOnlySpan<byte> buffer)
    {
        var copy = buffer;
        var typeId = ReadTypeId(ref copy);
        var value = Create(typeId);
        value.Deserialize(ref buffer);
        Debug.Assert(copy.Length == 0, "Buffer should be fully consumed after deserialization");
        return value;
    }

    public abstract Id TypeId { get; }

    #region ISizedSpanSerializable

    public void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        var typeId = ReadTypeId(ref buffer);
        if (typeId != TypeId)
        {
            throw new ArgumentException(
                $"Invalid command argument type: expected {TypeId}, got {typeId}"
            );
        }

        InternalDeserialize(ref buffer);

        Debug.Assert(buffer.Length == 0, "Buffer should be fully consumed after deserialization");
    }

    protected abstract void InternalDeserialize(ref ReadOnlySpan<byte> buffer);

    public void Serialize(ref Span<byte> buffer)
    {
        WriteTypeId(ref buffer, TypeId);
        InternalSerialize(ref buffer);
    }

    protected abstract void InternalSerialize(ref Span<byte> buffer);

    public int GetByteSize() => GetSizeOfTypeId(TypeId) + InternalGetByteSize();

    protected abstract int InternalGetByteSize();

    #endregion
}
