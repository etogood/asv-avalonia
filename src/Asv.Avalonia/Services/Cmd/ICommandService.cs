using Avalonia.Input;
using ObservableCollections;

namespace Asv.Avalonia;

public interface ICommandService
{
    IEnumerable<ICommandInfo> Commands { get; }
    IAsyncCommand? CreateCommand(string commandId);
    ICommandHistory CreateHistory(IRoutable owner);
    bool CanExecuteCommand(string commandId, IRoutable context, out IRoutable? target);
    void ChangeHotKey(string commandId, KeyGesture? hotKey);
    bool TryGetCommand(KeyGesture gesture, IRoutable context, out IAsyncCommand? command, out IRoutable? target);
}


