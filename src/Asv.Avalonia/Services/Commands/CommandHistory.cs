using System.Buffers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using R3;

namespace Asv.Avalonia;

public class CommandHistory : ICommandHistory
{
    private readonly Dictionary<string, IViewModel> _context = new();
    private readonly Stack<(IUndoableCommand, string)> _undoStack = new();
    private readonly Stack<(IUndoableCommand, string)> _redoStack = new();

    public CommandHistory(string id)
    {
        Id = id;
        Undo = new ReactiveCommand((_, token) => UndoAsync(token));
        Redo = new ReactiveCommand((_, token) => RedoAsync(token));
    }

    public string Id { get; }
    public IDisposable Register(IViewModel context)
    {
        _context.Add(context.Id, context);
        return Disposable.Create(context, Unregister);
    }

    public void Unregister(IViewModel context)
    {
        _context.Remove(context.Id);
    }

    public ReactiveCommand Undo { get; }
    public async ValueTask UndoAsync(CancellationToken cancel = default)
    {
        if (_undoStack.TryPop(out var command)
            && _context.TryGetValue(command.Item2, out var context))
        {
            await command.Item1.Undo(context, cancel);
            _redoStack.Push(command);
        }
    }

    public ReactiveCommand Redo { get; }
    public async ValueTask RedoAsync(CancellationToken cancel = default)
    {
        if (_redoStack.TryPop(out var command)
            && _context.TryGetValue(command.Item2, out var context))
        {
            await command.Item1.Redo(context, cancel);
            _undoStack.Push(command);
        }
    }

    public ValueTask Execute(ICommandBase command, IViewModel context, object? param, CancellationToken cancel = default)
    {
        throw new NotImplementedException();
    }

    public async ValueTask Execute(ICommandBase command, IViewModel context, CancellationToken cancel = default)
    {
        await command.Execute(context, cancel);
        if (command is IUndoableCommand withUndo)
        {
            _undoStack.Push((withUndo, context.Id));
        }
    }

    public void Load(string[] data)
    {
        foreach (var command in data)
        {
            var commandUri = new Uri(command);
                
        }
    }
}