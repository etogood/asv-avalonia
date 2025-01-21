using Avalonia.Controls.ApplicationLifetimes;

namespace Asv.Avalonia;

public class SingleViewAppShell : Shell
{
    public SingleViewAppShell(ISingleViewApplicationLifetime lifetime, IContainerHost containerHost)
        : base(containerHost)
    {
        throw new System.NotImplementedException();
    }

    protected override void InternalAddPageToMainTab(IShellPage export)
    {
        throw new NotImplementedException();
    }
}