using System;
using System.Collections.Generic;
using System.Composition;
using Asv.Common;
using Material.Icons;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using R3;

namespace Asv.Avalonia.Example;

[ExportControlExamples(PageId)]
public class InfoBoxControlsPageViewModel : ControlsGallerySubPage
{
    public const string PageId = "info_box_controls";
    public const MaterialIconKind PageIcon = MaterialIconKind.InfoBox;
    private readonly ReactiveProperty<string?> _infoBoxMessage;

    private readonly ReactiveProperty<string?> _infoBoxTitle;

    public InfoBoxControlsPageViewModel()
        : this(NullLoggerFactory.Instance)
    {
        DesignTime.ThrowIfNotDesignMode();
    }

    [ImportingConstructor]
    public InfoBoxControlsPageViewModel(ILoggerFactory loggerFactory)
        : base(PageId, loggerFactory)
    {
        Severity = new BindableReactiveProperty<InfoBarSeverity>(
            InfoBarSeverity.Informational
        ).DisposeItWith(Disposable);

        _infoBoxTitle = new ReactiveProperty<string?>(
            RS.InfoBoxControlsPageViewModel_Example_Title
        ).DisposeItWith(Disposable);
        _infoBoxMessage = new ReactiveProperty<string?>(
            RS.InfoBoxControlsPageViewModel_Example_Message
        ).DisposeItWith(Disposable);

        InfoBoxTitle = new HistoricalStringProperty(
            nameof(InfoBoxTitle),
            _infoBoxTitle,
            loggerFactory,
            this
        ).DisposeItWith(Disposable);
        InfoBoxMessage = new HistoricalStringProperty(
            nameof(InfoBoxMessage),
            _infoBoxMessage,
            loggerFactory,
            this
        ).DisposeItWith(Disposable);
    }

    public HistoricalStringProperty InfoBoxTitle { get; }
    public HistoricalStringProperty InfoBoxMessage { get; }

    // TODO: use historical property after it is implemented
    public BindableReactiveProperty<InfoBarSeverity> Severity { get; }
    public InfoBarSeverity[] Severities { get; } = Enum.GetValues<InfoBarSeverity>();

    public override IEnumerable<IRoutable> GetRoutableChildren()
    {
        yield return InfoBoxTitle;
        yield return InfoBoxMessage;

        foreach (var child in base.GetRoutableChildren())
        {
            yield return child;
        }
    }

    public override IExportInfo Source => SystemModule.Instance;
}
