using System.Collections.Generic;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Asv.Avalonia.Example;

[ExportPage(PageId)]
public class DocumentPageViewModel : PageViewModel<DocumentPageViewModel>
{
    public const string PageId = "document";

    public DocumentPageViewModel()
        : this(DesignTime.CommandService, DesignTime.LoggerFactory)
    {
        DesignTime.ThrowIfNotDesignMode();

        Title = RS.DocumentPageViewModel_Title;
    }

    [ImportingConstructor]
    public DocumentPageViewModel(ICommandService cmd, ILoggerFactory loggerFactory)
        : base(PageId, cmd, loggerFactory)
    {
        Title = RS.DocumentPageViewModel_Title;
    }

    protected override DocumentPageViewModel GetContext()
    {
        return this;
    }

    protected override void AfterLoadExtensions() { }

    public override IEnumerable<IRoutable> GetRoutableChildren()
    {
        return [];
    }

    public override IExportInfo Source => SystemModule.Instance;
}
