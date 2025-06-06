namespace Asv.Avalonia;

public class TreeVisitorEvent(IRoutable source)
    : AsyncRoutedEvent(source, RoutingStrategy.Tunnel) { }
