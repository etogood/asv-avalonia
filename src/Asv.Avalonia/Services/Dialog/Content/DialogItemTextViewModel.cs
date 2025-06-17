using Avalonia.Controls;
using Microsoft.Extensions.Logging;

namespace Asv.Avalonia;

public class DialogItemTextViewModel : DialogViewModelBase
{
    public const string DialogId = "dialog.item.text";

    public DialogItemTextViewModel(ILoggerFactory loggerFactory)
        : base(DialogId, loggerFactory)
    {
        if (Design.IsDesignMode)
        {
            Message = "Example";
        }
    }

    public required string Message { get; set; }

    public override IEnumerable<IRoutable> GetRoutableChildren()
    {
        return [];
    }
}
