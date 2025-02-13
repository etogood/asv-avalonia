using System.Composition;

namespace Asv.Avalonia;

public interface ICommandFactory
{
    ICommandInfo Info { get; }
    bool CanExecute(IRoutable context, IPersistable? parameter);
    ValueTask<IPersistable?> Execute(
        IRoutable context,
        IPersistable? parameter,
        CancellationToken cancel = default
    );
    ValueTask Undo(IRoutable context, IPersistable? parameter, CancellationToken cancel = default);
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class ExportCommandAttribute() : ExportAttribute(typeof(ICommandFactory)) { }
