using Asv.IO;

namespace Asv.Avalonia;

public partial class CommandArg
{
    public static CommandArg EmptyString => new StringArg(string.Empty);
}

public class StringArg(string value) : CommandArg
{
    public static CommandArg Create() => new StringArg(string.Empty);

    public string Value { get; private set; } = value;

    public override Id TypeId => Id.String;

    protected override void InternalDeserialize(ref ReadOnlySpan<byte> buffer) =>
        Value = BinSerialize.ReadString(ref buffer);

    protected override void InternalSerialize(ref Span<byte> buffer) =>
        BinSerialize.WriteString(ref buffer, Value);

    protected override int InternalGetByteSize() => BinSerialize.GetSizeForString(Value);

    public override string ToString() => Value;
}
