using Avalonia.Controls;
using Microsoft.Extensions.Logging;
using R3;

namespace Asv.Avalonia;

public class DialogItemTextBoxViewModel : DialogViewModelBase
{
    public const string DialogId = "dialog.item.textbox";

    public DialogItemTextBoxViewModel(ILoggerFactory loggerFactory)
        : base(DialogId, loggerFactory)
    {
        Input = new BindableReactiveProperty<string?>();
        if (Design.IsDesignMode)
        {
            Message = "Example";
        }
    }

    public BindableReactiveProperty<string?> Input { get; }
    public string? Message { get; set; }

    public override IEnumerable<IRoutable> GetRoutableChildren()
    {
        return [];
    }
}
