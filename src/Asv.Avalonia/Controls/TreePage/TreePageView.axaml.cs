using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Asv.Avalonia;

[ExportViewFor(typeof(IDesignTimeTreePage))]
public partial class TreePageView : UserControl
{
    public TreePageView()
    {
        InitializeComponent();
    }
}