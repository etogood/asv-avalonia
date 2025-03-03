using System.Composition;

namespace Asv.Avalonia;

[ExportPage(PageId)]
public class HomePageViewModel : PageViewModel<HomePageViewModel>
{
    public const string PageId = "home";

    public HomePageViewModel()
        : this(DesignTime.CommandService)
    {
        DesignTime.ThrowIfNotDesignMode();

        Title.OnNext(RS.HomePageViewModel_Title);
    }

    [ImportingConstructor]
    public HomePageViewModel(ICommandService cmd)
        : base(PageId, cmd)
    {
        Title.OnNext(RS.HomePageViewModel_Title);
    }

    protected override HomePageViewModel GetContext()
    {
        return this;
    }

    protected override void AfterLoadExtensions()
    {
        // do nothing
    }

    public override IEnumerable<IRoutable> GetRoutableChildren()
    {
        return [];
    }

    public override IExportInfo Source => SystemModule.Instance;
}
