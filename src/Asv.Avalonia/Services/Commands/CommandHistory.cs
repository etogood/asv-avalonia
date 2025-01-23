using System.Buffers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using R3;

namespace Asv.Avalonia;

public class CommandHistory : ICommandHistory
{
    private readonly ICommandService _svc;
    private readonly Dictionary<string, IViewModel> _context = new();
    private readonly Stack<(IUndoableCommand, string)> _undoStack = new();
    private readonly Stack<(IUndoableCommand, string)> _redoStack = new();

    public CommandHistory(string id, ICommandService svc)
    {
        _svc = svc;
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

    public ValueTask Execute(string commandId, IViewModel context, IMemento? param, CancellationToken cancel = default)
    {
        return _svc.Create(commandId)?.Execute(context, param, cancel) ?? ValueTask.CompletedTask;
    }
    
    public void Load(string[] data)
    {
        foreach (var command in data)
        {
            var commandUri = new Uri(command);
                
        }
    }
}