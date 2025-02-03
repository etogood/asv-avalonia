using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Interactivity;
using R3;

namespace Asv.Avalonia;

public class DesktopShellViewModel : ShellViewModel
{
    private readonly ICommandService _commandService;

    public DesktopShellViewModel(
        IClassicDesktopStyleApplicationLifetime lifetime,
        ICommandService commandService,
        IContainerHost containerHost
    )
        : base(containerHost)
    {
        _commandService = commandService;
        lifetime.MainWindow = new ShellWindow { DataContext = this };
        InputElement.KeyDownEvent.AddClassHandler<TopLevel>(OnKeyDownCustom, handledEventsToo: true);
        InputElement.GotFocusEvent.AddClassHandler<TopLevel>(FocusChanged, handledEventsToo: true);
        lifetime.MainWindow.Show();
    }

    private void FocusChanged<TTarget>(TTarget arg1, GotFocusEventArgs arg2) 
        where TTarget : Interactive
    {
        
    }

    private void OnKeyDownCustom(TopLevel source, KeyEventArgs keyEventArgs)
    {
        if (keyEventArgs.KeyModifiers == KeyModifiers.None)
        {
            // we don't want to handle key events without modifiers
            return;
        }

        if (_commandService.TryGetCommand(new KeyGesture(keyEventArgs.Key, keyEventArgs.KeyModifiers), SelectedControl.CurrentValue, out var command, out var target))
        {
            if (command == null || target == null)
            {
                return;
            }

            target.ExecuteCommand(command.Info.Id, null);
            keyEventArgs.Handled = true;
        }
    }

    protected override ValueTask CloseAsync(CancellationToken cancellationToken)
    {
        if (Application.Current != null && Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
        {
            lifetime.Shutdown();
        }

        return ValueTask.CompletedTask;
    }
}
