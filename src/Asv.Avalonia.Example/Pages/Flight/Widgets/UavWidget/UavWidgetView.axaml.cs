using Avalonia.Controls;

namespace Asv.Avalonia.Example;

[ExportViewFor(typeof(UavWidgetViewModel))]
public partial class UavWidgetView : UserControl
{
    public UavWidgetView()
    {
        InitializeComponent();
    }
}
