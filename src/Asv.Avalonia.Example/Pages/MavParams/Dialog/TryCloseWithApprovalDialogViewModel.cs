using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Asv.Avalonia.Example;

public class TryCloseWithApprovalDialogViewModel(ILoggerFactory loggerFactory)
    : DialogViewModelBase(DialogId, loggerFactory)
{
    public const string DialogId = "params.close-with-approval-dialog.text";

    public string Message { get; } = RS.ParamPageViewModel_DataLossDialog_Content;

    public override IEnumerable<IRoutable> GetRoutableChildren()
    {
        return [];
    }
}
