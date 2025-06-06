namespace Asv.Avalonia.Progress;

public class ProgressMessage(double progress, string message)
{
    public string Message => message;
    public double Progress => progress;
}
