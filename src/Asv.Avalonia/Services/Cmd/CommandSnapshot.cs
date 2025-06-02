using Asv.IO;

namespace Asv.Avalonia;

public sealed class CommandSnapshot(
    string commandId,
    NavigationPath contextPath,
    CommandArg newValue,
    CommandArg? oldValue
) : ISizedSpanSerializable
{
    private CommandArg _newValue = newValue;
    private CommandArg? _oldValue = oldValue;
    public string CommandId { get; private set; } = commandId;
    public NavigationPath ContextPath { get; private set; } = contextPath;

    public CommandArg NewValue => _newValue;

    public CommandArg? OldValue => _oldValue;

    public override string ToString()
    {
        return $"{CommandId}[{ContextPath}]:({OldValue})=>({NewValue}))";
    }

    public void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        CommandId = BinSerialize.ReadString(ref buffer);
        ContextPath = new NavigationPath(ref buffer);
        _newValue = CommandArg.Create(ref buffer);
        if (BinSerialize.ReadBool(ref buffer))
        {
            _oldValue = CommandArg.Create(ref buffer);
        }
    }

    public void Serialize(ref Span<byte> buffer)
    {
        BinSerialize.WriteString(ref buffer, CommandId);
        ContextPath.Serialize(ref buffer);
        NewValue.Serialize(ref buffer);
        if (OldValue is null)
        {
            BinSerialize.WriteBool(ref buffer, false);
        }
        else
        {
            BinSerialize.WriteBool(ref buffer, true);
            OldValue.Serialize(ref buffer);
        }
    }

    public int GetByteSize()
    {
        return BinSerialize.GetSizeForString(CommandId)
            + ContextPath.GetByteSize()
            + NewValue.GetByteSize()
            + (OldValue is null ? sizeof(bool) : sizeof(bool) + OldValue.GetByteSize());
    }
}
