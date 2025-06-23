using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Asv.Avalonia;

[ExportViewFor<DigitRttBoxViewModel>]
public partial class DigitRttBoxView : UserControl
{
    public DigitRttBoxView()
    {
        InitializeComponent();
    }
}
