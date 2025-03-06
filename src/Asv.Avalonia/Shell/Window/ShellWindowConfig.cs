namespace Asv.Avalonia;

public class ShellWindowConfig
{
    public double Width { get; set; }
    public double Height { get; set; }
    public int PositionX { get; set; }
    public int PositionY { get; set; }
    public bool IsMaximized { get; set; }

    public override string ToString() =>
        $"MAX:{IsMaximized}, X:{PositionX}, Y:{PositionY}, W:{Width:F0}, H:{Height:F0}";
}
