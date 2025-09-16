using Avalonia.Controls;

namespace Asv.Avalonia;

public partial class DockWindow : Window
{
    public required string Id { get; init; }

    public DockWindow()
    {
        InitializeComponent();
    }
}
