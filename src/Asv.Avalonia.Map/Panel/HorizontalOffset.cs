using Asv.Common;
using Avalonia.Markup.Xaml;

namespace Asv.Avalonia.Map;

public class HorizontalOffsetExtension(MapOffsetXEnum offsetType) : MarkupExtension
{
    public double Offset { get; set; } = 0.0;

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return new HorizontalOffset(offsetType, Offset);
    }
}

public readonly struct HorizontalOffset(MapOffsetXEnum offsetType, double offset)
{
    public MapOffsetXEnum OffsetType => offsetType;
    public double Offset => offset;

    public double CalculateOffset(double boundsWidth)
    {
        return OffsetType switch
        {
            MapOffsetXEnum.Left => Offset,
            MapOffsetXEnum.Center => Offset - boundsWidth / 2,
            MapOffsetXEnum.Right => Offset - boundsWidth,
            _ => throw new ArgumentOutOfRangeException(),
        };
    }
}

/// <summary>
/// Enumeration for X-axis offset values.
/// </summary>
public enum MapOffsetXEnum
{
    /// <summary>
    /// Represents the offset value for the X-axis of an element to be positioned on the left side.
    /// </summary>
    Left,

    /// <summary>
    /// Enumeration for X-axis offset values.
    /// </summary>
    Center,

    /// <summary>
    /// Enumeration for X-axis offset values.
    /// </summary>
    Right,
}
