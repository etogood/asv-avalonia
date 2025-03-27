using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Asv.Avalonia;

[ExportViewFor(typeof(GroupTreePageItemViewModel))]
public partial class GroupTreePageItemView : UserControl
{
    public GroupTreePageItemView()
    {
        InitializeComponent();
    }
}
