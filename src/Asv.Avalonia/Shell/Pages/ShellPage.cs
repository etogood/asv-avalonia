using R3;

namespace Asv.Avalonia;

public abstract class ShellPage : RoutableViewModel, IShellPage
{
    protected ShellPage(string id, ICommandService cmd)
        : base(id)
    {
        History = cmd.CreateHistory(this);
    }

    public abstract IReadOnlyBindableReactiveProperty<string> Title { get; }
    public ICommandHistory History { get; }

    protected override ValueTask InternalCatchEvent(AsyncRoutedEvent e)
    {
        if (e is ExecuteCommandEvent cmd)
        {
            return History.Execute(cmd.CommandId, cmd.Source, cmd.CommandParameter);
        }

        return ValueTask.CompletedTask;
    }
}