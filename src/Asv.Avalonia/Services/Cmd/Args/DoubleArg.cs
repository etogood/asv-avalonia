using Asv.IO;

namespace Asv.Avalonia;

public partial class CommandArg
{
    public static CommandArg Double(double value)
    {
        return new DoubleArg(value);
    }

    public static explicit operator double(CommandArg value)
    {
        if (value is DoubleArg boolArg)
        {
            return boolArg.Value;
        }

        throw new InvalidCastException($"Cannot cast {value.GetType().Name} to {nameof(Double)}");
    }

    public static implicit operator CommandArg(double value)
    {
        return new DoubleArg(value);
    }
}

public class DoubleArg(double value) : CommandArg
{
    public static CommandArg Create() => new DoubleArg(double.NaN);

    private double _value = value;
    public double Value => _value;

    public override Id TypeId => Id.Double;

    protected override void InternalDeserialize(ref ReadOnlySpan<byte> buffer) =>
        BinSerialize.ReadDouble(ref buffer, ref _value);

    protected override void InternalSerialize(ref Span<byte> buffer) =>
        BinSerialize.WriteDouble(ref buffer, in _value);

    protected override int InternalGetByteSize() => sizeof(double);

    public override string ToString() => $"{Value:G}";
}
