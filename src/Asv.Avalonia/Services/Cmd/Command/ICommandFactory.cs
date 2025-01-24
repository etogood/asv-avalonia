using System.Composition;

namespace Asv.Avalonia;

public interface ICommandFactory
{
    ICommandInfo Info { get; }
    IAsyncCommand Create();
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class ExportCommandAttribute()
    : ExportAttribute(typeof(ICommandFactory))
{
 
}