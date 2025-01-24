namespace Asv.Avalonia;

public class ExecuteCommandEvent(IRoutableViewModel source, string commandId, IPersistable? commandParameter)
    : AsyncRoutedEvent(source, RoutingEventStrategy.Bubble)
{
    public string CommandId { get; } = commandId;
    public IPersistable? CommandParameter { get; } = commandParameter;
}

public class FocusedEvent(IRoutableViewModel source)
    : AsyncRoutedEvent(source, RoutingEventStrategy.Bubble)
{
    
}

public static class ExecuteCommandEventMixin
{
    public static ValueTask ExecuteCommand(this IRoutableViewModel src, string commandId, IPersistable? commandParameter)
    {
        return src.Rise(new ExecuteCommandEvent(src, commandId, commandParameter));
    }
}