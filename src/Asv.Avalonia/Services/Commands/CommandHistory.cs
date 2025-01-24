using System.Buffers;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using R3;

namespace Asv.Avalonia;

public class CommandHistory : ICommandHistory
{
    private readonly ICommandService _cmd;
    private readonly Stack<(IAsyncUndoRedoCommand, string[])> _undoStack = new();
    private readonly Stack<(IAsyncUndoRedoCommand, string[])> _redoStack = new();

    public CommandHistory(IRoutableViewModel owner, ICommandService cmd)
    {
        Owner = owner;
        _cmd = cmd;
        Undo = new ReactiveCommand((_, token) => UndoAsync(token));
        Redo = new ReactiveCommand((_, token) => RedoAsync(token));
    }

    public IRoutableViewModel Owner { get; }
    public ReactiveCommand Undo { get; }
    public async ValueTask UndoAsync(CancellationToken cancel = default)
    {
        if (_undoStack.TryPop(out var command))
        {
            var context = await GetContext(Owner, command.Item2);
            await command.Item1.Undo(context, cancel);
            _redoStack.Push(command);
        }
    }

    public ReactiveCommand Redo { get; }
    public async ValueTask RedoAsync(CancellationToken cancel = default)
    {
        if (_redoStack.TryPop(out var command))
        {
            var context = await GetContext(Owner, command.Item2);
            await command.Item1.Redo(context, cancel);
            _undoStack.Push(command);
        }
    }

    private ValueTask<IRoutableViewModel> GetContext(IRoutableViewModel owner, string[] path)
    {
        if (path.Length == 0 || path[0] != Owner.Id)
        {
            // this command is not for us
            return ValueTask.FromResult(Owner);
        }

        return Owner.NavigateTo(path[1..]);
    }

    public ValueTask Execute(string commandId, IRoutableViewModel context, IPersistable? param, CancellationToken cancel = default)
    {
        var cmd = _cmd.Create(commandId);
        if (cmd == null)
        {
            return ValueTask.CompletedTask;
        }

        if (cmd is IAsyncUndoRedoCommand undoable)
        {
            var contextPath = context.GetAllFrom(Owner).Select(x => x.Id).ToArray();
            _undoStack.Push((undoable, contextPath));
            _redoStack.Clear();
        }

        return cmd.Execute(context, param, cancel);
    }

}