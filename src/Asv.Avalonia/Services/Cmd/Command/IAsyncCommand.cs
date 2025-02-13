using System.Buffers;
using R3;

namespace Asv.Avalonia;

public interface IAsyncCommand
{
    ICommandInfo Info { get; }
    ValueTask Execute(CancellationToken cancel = default);
}
