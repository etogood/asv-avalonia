using System;
using System.Collections.Generic;
using R3;

namespace Asv.Avalonia.Example.ViewModels;

public class MainViewModel : DisposableViewModel
{
    public MainViewModel()
        : base("shell")
    {
        History = new HistoryStack();
        Property1 = new HistoryProperty(History, "property1");
        Property2 = new HistoryProperty(History, "property2");
        Undo = new ReactiveCommand(_ => History.Undo());
        Redo = new ReactiveCommand(_ => History.Redo());
    }
    
    public HistoryStack History { get; }
    public HistoryProperty Property2 { get; }
    public HistoryProperty Property1 { get; }
    public ReactiveCommand Undo { get; }
    public ReactiveCommand Redo { get; }
}

public class HistoryStack 
{
    private readonly Stack<ChangeStringCommand> _undoStack = new();
    private readonly Stack<ChangeStringCommand> _redoStack = new();
    
    public HistoryStack()
    {
        
    }

    public IDisposable Register(HistoryProperty property)
    {
        
    }
    
    public void Execute(ChangeStringCommand command)
    {
        command.Execute();
        _undoStack.Push(command);
        _redoStack.Clear(); // Clear redo stack on new action
    }

    public void Undo()
    {
        if (_undoStack.TryPop(out var command))
        {
            command.Undo();
            _redoStack.Push(command);
        }
    }

    public void Redo()
    {
        if (_redoStack.TryPop(out var command))
        {
            command.Execute();
            _undoStack.Push(command);
        }
    }
}

public class HistoryProperty : ViewModelBase
{
    private readonly HistoryStack _commandManager;
    private bool _internalChange;

    public BindableReactiveProperty<string> User { get; } = new();
    public ReactiveProperty<string> Model { get; } = new();
    public BindableReactiveProperty<bool> IsSelected { get; } = new();
    
    public HistoryProperty(HistoryStack commandManager, string id)
        : base(id)
    {
        _commandManager = commandManager;
        _internalChange = true;
        User.Subscribe(OnChangedByUser);
        _internalChange = false;
        Model.Subscribe(OnChangeByModel);
    }

    private void OnChangedByUser(string value)
    {
        if (_internalChange)
        {
            return;
        }
        
        _commandManager.Execute(new ChangeStringCommand(this));
    }
    
    private void OnChangeByModel(string value)
    {
        _internalChange = true;
        User.OnNext(value);
        _internalChange = false;
    }

    protected override void Dispose(bool disposing)
    {
        // ignore
    }
}

public class ChangeStringCommand
{
    private readonly HistoryProperty _context;
    private string _oldValue;
    private string _newValue;

    public ChangeStringCommand(HistoryProperty context)
    {
        _context = context;
        _oldValue = context.Model.Value;
        _newValue = context.User.Value;
    }

    public void Execute()
    {
        _context.Model.OnNext(_newValue);
    }
    
    public void Undo()
    {
        _context.IsSelected.OnNext(true);
        _context.Model.OnNext(_oldValue);
    }

}