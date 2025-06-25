using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Asv.Avalonia;

[ExportViewFor<SingleRttBoxViewModel>]
public partial class SingleRttBoxView : UserControl
{
    public SingleRttBoxView()
    {
        InitializeComponent();
    }
}
