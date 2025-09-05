using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Asv.Avalonia.IO;

[ExportViewFor(typeof(ConnectionRateStatusViewModel))]
public partial class ConnectionRateStatusView : UserControl
{
    public ConnectionRateStatusView()
    {
        InitializeComponent();
    }
}
