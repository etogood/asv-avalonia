using Avalonia.Controls;

namespace Asv.Avalonia;

[ExportViewFor(typeof(SettingsCommandsViewModel))]
public partial class SettingsCommandsView : UserControl
{
    public SettingsCommandsView()
    {
        InitializeComponent();
    }
}
