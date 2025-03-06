using Avalonia.Media;
using Material.Icons;

namespace Asv.Avalonia;

public interface IHeadlinedViewModel : IRoutable
{
    /// <summary>
    /// Gets or sets the icon associated with the view model.
    /// </summary>
    MaterialIconKind? Icon { get; set; }

    /// <summary>
    /// Gets or sets the brush for the icon.
    /// </summary>
    IBrush? IconBrush { get; set; }

    /// <summary>
    /// Gets or sets the header (title) of the view model.
    /// </summary>
    string? Header { get; set; }

    /// <summary>
    /// Gets or sets the description of the view model.
    /// </summary>
    string? Description { get; set; }

    /// <summary>
    /// Gets or sets the order of the view model.
    /// </summary>
    int Order { get; set; }
}
