using Avalonia.Controls.ApplicationLifetimes;

namespace Asv.Avalonia;

public class ClassicDesktopShell : Shell
{
    public ClassicDesktopShell(IClassicDesktopStyleApplicationLifetime lifetime, IContainerHost containerHost)
        : base(containerHost)
    {
        lifetime.MainWindow = new ShellWindow { DataContext = this };
    }

    protected override void InternalAddPageToMainTab(IShellPage export)
    {
        throw new NotImplementedException();
    }
}
