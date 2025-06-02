namespace Asv.Avalonia;

public class ExecuteCommandEvent(IRoutable source, string commandId, CommandArg commandArg)
    : AsyncRoutedEvent(source)
{
    public string CommandId { get; } = commandId;
    public CommandArg CommandArg { get; } = commandArg;
}

public static class ExecuteCommandEventMixin
{
    public static ValueTask ExecuteCommand(
        this IRoutable src,
        string commandId,
        CommandArg commandArg
    )
    {
        return src.Rise(new ExecuteCommandEvent(src, commandId, commandArg));
    }

    public static ValueTask ExecuteCommand(this IRoutable src, string commandId)
    {
        return src.Rise(new ExecuteCommandEvent(src, commandId, CommandArg.Null));
    }
}
