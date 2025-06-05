using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Asv.Avalonia;

[ExportViewFor(typeof(SearchBoxViewModel))]
public partial class SearchBoxView : UserControl
{
    public SearchBoxView()
    {
        InitializeComponent();
    }

    
}