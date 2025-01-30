using System.Composition;

namespace Asv.Avalonia;

[ExportSettings(PageId)]
public class SettingsUnitsViewModel : RoutableViewModel, ISettingsSubPage
{
    [ImportingConstructor]
    public SettingsUnitsViewModel()
        : base(PageId)
    {
    }

    public const string PageId = "units";
    public ValueTask Init(ISettingsPage context)
    {
        return ValueTask.CompletedTask;
    }

    public override ValueTask<IRoutable> NavigateTo(string id)
    {
        return ValueTask.FromResult<IRoutable>(this);
    }
}