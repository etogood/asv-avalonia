namespace Asv.Avalonia;

public class DesignTimeShell : Shell
{
    public DesignTimeShell()
        : base(NullContainerHost.Instance)
    {
    }

    protected override void InternalAddPageToMainTab(IShellPage export)
    {
        // ignore
    }
}