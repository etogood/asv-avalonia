using Avalonia.Controls;

namespace Asv.Avalonia.LogViewer;

[ExportViewFor(typeof(LogViewerViewModel))]
public partial class LogViewerView : UserControl
{
    public LogViewerView()
    {
        InitializeComponent();
    }
}
