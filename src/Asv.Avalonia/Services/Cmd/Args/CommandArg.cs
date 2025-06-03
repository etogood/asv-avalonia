using System.Diagnostics;
using System.Runtime.CompilerServices;
using Asv.IO;
using Newtonsoft.Json;

namespace Asv.Avalonia;

public abstract partial class CommandArg : ISizedSpanSerializable, IJsonSerializable
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

    public static Id? ReadTypeId(JsonReader reader)
    {
        if (reader.Read() == false)
        {
            throw new JsonSerializationException($"{nameof(ReadTypeId)} cannot be null.");
        }

        if (reader.TokenType == JsonToken.Null)
        {
            return null;
        }

        if (reader.TokenType != JsonToken.String)
        {
            throw new JsonSerializationException(
                $"{nameof(ReadTypeId)} expected a string token, but got {reader.TokenType}."
            );
        }

        var name = (string)(
            reader.Value
            ?? throw new JsonSerializationException($"{nameof(ReadTypeId)} cannot be null.")
        );

        return Enum.Parse<Id>(name);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteTypeId(ref Span<byte> buffer, Id typeId) =>
        BinSerialize.WritePackedUnsignedInteger(ref buffer, (uint)typeId);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteTypeId(JsonWriter writer, Id typeId) =>
        writer.WriteValue(typeId.ToString("G"));

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

    public static CommandArg? Create(JsonReader reader)
    {
        if (reader.Read() == false)
        {
            throw new JsonSerializationException($"{nameof(Create)} cannot be null.");
        }

        if (reader.TokenType == JsonToken.Null)
        {
            return null;
        }

        if (reader.TokenType != JsonToken.StartArray)
        {
            throw new JsonSerializationException(
                $"{nameof(Create)} expected a StartArray token, but got {reader.TokenType}."
            );
        }

        var typeId = ReadTypeId(reader);
        if (typeId == null)
        {
            return null;
        }

        var value = Create(typeId.Value);
        value.InternalDeserialize(reader);

        if (reader.Read() == false || reader.TokenType != JsonToken.EndArray)
        {
            throw new JsonSerializationException(
                $"{nameof(Create)} expected an EndArray token, but got {reader.TokenType}."
            );
        }

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

    #region IJsonSerializable

    public void Serialize(JsonWriter writer)
    {
        writer.WriteStartArray();
        WriteTypeId(writer, TypeId);
        InternalSerialize(writer);
        writer.WriteEndArray();
    }

    protected abstract void InternalDeserialize(JsonReader reader);
    protected abstract void InternalSerialize(JsonWriter writer);

    public void Deserialize(JsonReader reader)
    {
        throw new NotImplementedException(
            $"Use static factory method {nameof(CommandArg)}.{nameof(Create)}({nameof(JsonReader)} reader) instead."
        );
    }

    #endregion
}
