using System.Composition;

namespace Asv.Avalonia;

public interface IExtensionFor<in T> : IDisposable
{
    ValueTask Extend(T viewModel);
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class ExportExtensionForAttribute<T>()
    : ExportAttribute(typeof(IExtensionFor<T>));