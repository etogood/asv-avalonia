using System.Composition;

namespace Asv.Avalonia;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class ExportStatusItemAttribute() : ExportAttribute(Contract, typeof(IStatusItem))
{
    public const string Contract = "shell.status";
}
