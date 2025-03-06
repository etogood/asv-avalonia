using System.Windows.Input;

namespace Asv.Avalonia;

public class ActionViewModel(string id) : HeadlinedViewModel(id), IActionViewModel
{
    private ICommand? _command;
    private object? _commandParameter;
    private bool _isVisible = true;

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

    public bool IsVisible
    {
        get => _isVisible;
        set => SetField(ref _isVisible, value);
    }
}
