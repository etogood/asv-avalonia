using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Asv.Avalonia.Commands;

[ExportViewFor(typeof(SettingsCommandsViewModel))]
public partial class SettingsCommandsView : UserControl
{
    public SettingsCommandsView()
    {
        InitializeComponent();
    }
}
