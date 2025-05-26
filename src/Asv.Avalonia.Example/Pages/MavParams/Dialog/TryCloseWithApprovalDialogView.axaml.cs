using Avalonia.Controls;

namespace Asv.Avalonia.Example;

[ExportViewFor(typeof(TryCloseWithApprovalDialogViewModel))]
public partial class TryCloseWithApprovalDialogView : UserControl
{
    public TryCloseWithApprovalDialogView()
    {
        InitializeComponent();
    }
}
