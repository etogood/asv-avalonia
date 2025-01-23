namespace Asv.Avalonia;

public class ExecuteCommandEvent(IRoutableViewModel source, string commandId, IMemento? commandParameter)
    : AsyncRoutedEvent(source, RoutingEventStrategy.Bubble)
{
    public string CommandId { get; } = commandId;
    public IMemento? CommandParameter { get; } = commandParameter;
}

public static class ExecuteCommandEventMixin
{
    public static ValueTask ExecuteCommand(this IRoutableViewModel src, string commandId, IMemento? commandParameter)
    {
        return src.Rise(new ExecuteCommandEvent(src, commandId, commandParameter));
    }
}