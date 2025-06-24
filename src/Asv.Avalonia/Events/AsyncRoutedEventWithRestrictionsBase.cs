namespace Asv.Avalonia;

public abstract class AsyncRoutedEventWithRestrictionsBase(
    IRoutable source,
    RoutingStrategy routingStrategy
) : AsyncRoutedEvent(source, routingStrategy)
{
    private readonly List<Restriction> _restrictions = [];

    /// <summary>
    /// Gets the list of restrictions.
    /// </summary>
    public IReadOnlyList<Restriction> Restrictions => _restrictions;

    /// <summary>
    /// Adds a restriction.
    /// </summary>
    /// <param name="restriction">The restriction.</param>
    public void AddRestriction(Restriction restriction)
    {
        _restrictions.Add(restriction);
    }
}
