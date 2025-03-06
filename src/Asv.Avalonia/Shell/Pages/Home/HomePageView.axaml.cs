using Avalonia.Controls;

namespace Asv.Avalonia;

[ExportViewFor(typeof(HomePageViewModel))]
public partial class HomePageView : UserControl
{
    public HomePageView()
    {
        InitializeComponent();
    }
}
