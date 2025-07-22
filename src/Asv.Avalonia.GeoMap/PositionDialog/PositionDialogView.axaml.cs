using Asv.Avalonia;
using Asv.Avalonia.GeoMap;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Asv.Avalonia.GeoMap;

[ExportViewFor(typeof(PositionDialogViewModel))]
public partial class PositionDialogView : UserControl
{
    public PositionDialogView()
    {
        InitializeComponent();
    }
}
