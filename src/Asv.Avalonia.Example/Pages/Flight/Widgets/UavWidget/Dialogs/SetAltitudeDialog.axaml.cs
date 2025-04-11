using Avalonia.Controls;

namespace Asv.Avalonia.Example;

[ExportViewFor(typeof(SetAltitudeDialogViewModel))]
public partial class SetAltitudeDialog : UserControl
{
    public SetAltitudeDialog()
    {
        InitializeComponent();
    }
}
