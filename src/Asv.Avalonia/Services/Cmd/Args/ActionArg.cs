using System.Diagnostics;
using Asv.IO;
using Newtonsoft.Json;

namespace Asv.Avalonia;

public partial class CommandArg
{
    public static ActionArg AddAction(CommandArg options) => new(null, options, ActionArg.Kind.Add);

    public static ActionArg RemoveAction(string id) => new(id, null, ActionArg.Kind.Remove);

    public static ActionArg ChangeAction(string id, CommandArg options) =>
        new(id, options, ActionArg.Kind.Change);
}

public class ActionArg(string? subjectId, CommandArg? value, ActionArg.Kind action) : CommandArg
{
    public enum Kind : byte
    {
        Add = 0,
        Remove = 1,
        Change = 2,
    }

    protected internal static CommandArg CreateDefault() => new ActionArg(subjectId: null, null, Kind.Add);

    public string? SubjectId { get; private set; } = subjectId;
    public CommandArg? Value { get; private set; } = value;
    public Kind Action { get; private set; } = action;

    public override Id TypeId => Id.Action;

    protected override void InternalDeserialize(ref ReadOnlySpan<byte> buffer)
    {
        Action = (Kind)BinSerialize.ReadByte(ref buffer);
        switch (Action)
        {
            case Kind.Add:
                Value = CommandArg.Create(ref buffer);
                break;
            case Kind.Remove:
                SubjectId = BinSerialize.ReadString(ref buffer);
                break;
            case Kind.Change:
                SubjectId = BinSerialize.ReadString(ref buffer);
                Value = CommandArg.Create(ref buffer);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    protected override void InternalSerialize(ref Span<byte> buffer)
    {
        BinSerialize.WriteByte(ref buffer, (byte)Action);
        switch (Action)
        {
            case Kind.Add:
                Debug.Assert(Value != null, nameof(Value) + " != null");
                Value.Serialize(ref buffer);
                break;
            case Kind.Remove:
                Debug.Assert(SubjectId != null, nameof(SubjectId) + " != null");
                BinSerialize.WriteString(ref buffer, SubjectId);
                break;
            case Kind.Change:
                Debug.Assert(SubjectId != null, nameof(SubjectId) + " != null");
                Debug.Assert(Value != null, nameof(Value) + " != null");
                BinSerialize.WriteString(ref buffer, SubjectId);
                Value.Serialize(ref buffer);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    protected override int InternalGetByteSize()
    {
        var size = sizeof(byte); // Action
        switch (Action)
        {
            case Kind.Add:
                Debug.Assert(Value != null, nameof(Value) + " != null");
                size += Value.GetByteSize();
                break;
            case Kind.Remove:
                Debug.Assert(SubjectId != null, nameof(SubjectId) + " != null");
                size += BinSerialize.GetSizeForString(SubjectId);
                break;
            case Kind.Change:
                Debug.Assert(SubjectId != null, nameof(SubjectId) + " != null");
                Debug.Assert(Value != null, nameof(Value) + " != null");
                size += BinSerialize.GetSizeForString(SubjectId) + Value.GetByteSize();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return size;
    }

    protected override void InternalDeserialize(JsonReader reader)
    {
        if (reader.Read() == false || reader.TokenType != JsonToken.StartArray)
        {
            throw new JsonSerializationException("Expected start of array");
        }

        var actionValue =
            reader.ReadAsString()
            ?? throw new JsonSerializationException("Expected action type as string");
        if (!Enum.TryParse<Kind>(actionValue, true, out var action))
        {
            throw new JsonSerializationException($"Unknown action type: {actionValue}");
        }

        Action = action;

        SubjectId = reader.ReadAsString();
        Value = CommandArg.Create(reader);

        if (reader.Read() == false || reader.TokenType != JsonToken.EndArray)
        {
            throw new JsonSerializationException("Expected end of array");
        }
    }

    protected override void InternalSerialize(JsonWriter writer)
    {
        writer.WriteStartArray();
        writer.WriteValue(Action.ToString());
        writer.WriteValue(SubjectId);
        if (Value == null)
        {
            writer.WriteNull();
        }
        else
        {
            Value.Serialize(writer);
        }

        writer.WriteEndArray();
    }

    public override string ToString() => $"[{Action:G}] id:{SubjectId}, value:{Value}]";
}
