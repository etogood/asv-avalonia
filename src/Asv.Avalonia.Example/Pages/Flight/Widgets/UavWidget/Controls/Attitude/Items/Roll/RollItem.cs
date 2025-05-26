using System;
using Avalonia;

namespace Asv.Avalonia.Example;

public partial class RollItem : AvaloniaObject
{
    public RollItem(int angle)
    {
        Value = angle;
        Title =
            Math.Abs(angle) > 180 ? (360 - Math.Abs(angle)).ToString() : Math.Abs(angle).ToString();
    }
}
