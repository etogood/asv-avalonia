using System.Windows.Input;

namespace Asv.Avalonia;


public interface IQuickPick
{
    
}



public interface ICommandFactory
{
    
}

public interface ICommandManager
{
    ValueTask Execute(IUndoRedoCommand command);
}


public interface IAsvCommand
{
    
}


public interface IUndoRedoCommand : ICommand
{
    
}

public class UndoRedoCommand : IUndoRedoCommand
{
    public bool CanExecute(object? parameter)
    {
        throw new NotImplementedException();
    }

    public void Execute(object? parameter)
    {
        throw new NotImplementedException();
    }

    public event EventHandler? CanExecuteChanged;
}

