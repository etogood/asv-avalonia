using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Asv.Avalonia;

[ExportViewFor(typeof(LogViewerViewModel))]
public partial class LogViewerView : UserControl
{
    public LogViewerView()
    {
        InitializeComponent();
    }
    
}
