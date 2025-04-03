using Avalonia.Controls;

namespace Asv.Avalonia.Example;

[ExportViewFor(typeof(TestHistoryPropertiesPageViewModel))]
public partial class TestHistoryPropertiesPageView : UserControl
{
    public TestHistoryPropertiesPageView()
    {
        InitializeComponent();
    }
}
