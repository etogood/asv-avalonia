namespace Asv.Avalonia.Example.ViewModels;

public class MainViewModel : DisposableViewModel
{
    public MainViewModel()
        : base("shell") { }
#pragma warning disable CA1822 // Mark members as static
    public string Greeting => "Welcome to Avalonia!";
#pragma warning restore CA1822 // Mark members as static
}
