namespace Asv.Avalonia;

[ExportSettings(PageId)]
public class SettingsUnitsViewModel(string id) : RoutableViewModel(id), ISettingsSubPage
{
    public const string PageId = "units";
    public ValueTask Init(ISettingsPage context)
    {
        return ValueTask.CompletedTask;
    }
}