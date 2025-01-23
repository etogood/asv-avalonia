using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Asv.Avalonia;

[ExportViewFor(typeof(ThemePropertyViewModel))]
public partial class ThemePropertyView : UserControl
{
    public ThemePropertyView()
    {
        InitializeComponent();
    }
}