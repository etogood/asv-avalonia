using Asv.Avalonia.Routable;

namespace Asv.Avalonia;

/// <summary>
/// Represents an event triggered when validation happens.
/// </summary>
/// <param name="source">.</param>
/// <param name="sender">Object that was validated.</param>
/// <param name="validationResult">Result of the validation.</param>
public class ValidationEvent(IRoutable source, object sender, ValidationResult validationResult)
    : AsyncRoutedEvent(source, RoutingStrategy.Direct)
{
    public object Sender => sender;
    public ValidationResult ValidationResult => validationResult;
}
