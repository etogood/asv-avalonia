using Avalonia.Controls;

namespace Asv.Avalonia;

[ExportViewFor(typeof(SearchBoxViewModel))]
public partial class SearchBoxView : UserControl
{
    public SearchBoxView()
    {
        InitializeComponent();
    }
}
