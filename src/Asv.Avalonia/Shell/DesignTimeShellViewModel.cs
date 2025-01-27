namespace Asv.Avalonia;

public class DesignTimeShellViewModel : ShellViewModel
{
    public DesignTimeShellViewModel()
        : base(NullContainerHost.Instance)
    {
        OpenPage("settings");
    }

    protected override void InternalAddPageToMainTab(IPage export)
    {
        // ignore
    }
}