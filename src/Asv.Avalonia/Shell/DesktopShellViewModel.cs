using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;

namespace Asv.Avalonia;

public class DesktopShellViewModel : ShellViewModel
{
    public DesktopShellViewModel(
        IClassicDesktopStyleApplicationLifetime lifetime,
        IContainerHost containerHost
    )
        : base(containerHost)
    {
        lifetime.MainWindow = new ShellWindow { DataContext = this };
    }

    protected override ValueTask CloseAsync(CancellationToken cancellationToken)
    {
        if (Application.Current != null && Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
        {
            lifetime.Shutdown();
        }

        return ValueTask.CompletedTask;
    }

}
