using System.Collections.Generic;
using System.Composition;
using System.Threading.Tasks;
using Asv.Avalonia.GeoMap;
using Asv.Common;
using Material.Icons;
using Microsoft.Extensions.Logging;

namespace Asv.Avalonia.Example;

public interface IMapContext : IPage { }

[ExportPage(PageId)]
public class MapExamplePageViewModel : PageViewModel<IMapContext>, IMapContext
{
    public const string PageId = "MapExamplePage";

    public MapExamplePageViewModel()
        : this(DesignTime.CommandService, DesignTime.LoggerFactory)
    {
        DesignTime.ThrowIfNotDesignMode();

        Title = RS.MapExamplePageViewModel_Title;
    }

    [ImportingConstructor]
    public MapExamplePageViewModel(ICommandService cmd, ILoggerFactory loggerFactory)
        : base(PageId, cmd, loggerFactory)
    {
        Title = RS.MapExamplePageViewModel_Title;
        Anchors = new MapViewModel("Anthor1", loggerFactory);
        Anchors.Anchors.Add(
            new MapAnchor<IMapAnchor>("1", loggerFactory)
            {
                Icon = MaterialIconKind.Navigation,
                Location = new GeoPoint(53, 53, 0),
            }
        );
    }

    public override IEnumerable<IRoutable> GetRoutableChildren()
    {
        yield break;
    }

    protected override IMapContext GetContext()
    {
        return this;
    }

    protected override void AfterLoadExtensions()
    {
        // do nothing
    }

    public override IExportInfo Source => SystemModule.Instance;
    public MapViewModel Anchors { get; }
}
