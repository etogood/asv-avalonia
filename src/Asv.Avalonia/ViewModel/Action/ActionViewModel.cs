using System.Windows.Input;

namespace Asv.Avalonia;

public class ActionViewModel(string id) : HeadlinedViewModel(id), IActionViewModel
{
    private ICommand? _command;
    private object? _commandParameter;

    public ICommand? Command
    {
        get => _command;
        set => SetField(ref _command, value);
    }

    public object? CommandParameter
    {
        get => _commandParameter;
        set => SetField(ref _commandParameter, value);
    }
}
