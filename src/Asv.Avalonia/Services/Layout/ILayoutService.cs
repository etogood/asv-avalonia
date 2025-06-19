using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;

namespace Asv.Avalonia;

public interface ILayoutService
{
    TPocoType Get<TPocoType>(IRoutable source, Lazy<TPocoType> defaultValue)
        where TPocoType : class, new();

    void Set<TPocoType>(IRoutable source, TPocoType value)
        where TPocoType : class, new();

    TPocoType Get<TPocoType>(StyledElement source, Lazy<TPocoType> defaultValue)
        where TPocoType : class, new()
    {
        var control = source;
        while (control != null)
        {
            if (control.DataContext is IRoutable routable)
            {
                return this.Get<TPocoType>(routable, defaultValue);
                break;
            }

            // Try to find IRoutable DataContext in logical parent
            control = control.GetLogicalParent() as Control;
        }

        throw new InvalidOperationException(
            "No IRoutable DataContext found in the logical tree of the provided StyledElement."
        );
    }

    void Set<TPocoType>(StyledElement source, TPocoType value)
        where TPocoType : class, new()
    {
        var control = source;
        while (control != null)
        {
            if (control.DataContext is IRoutable routable)
            {
                Set(routable, value);
                break;
            }

            // Try to find IRoutable DataContext in logical parent
            control = control.GetLogicalParent() as Control;
        }

        throw new InvalidOperationException(
            "No IRoutable DataContext found in the logical tree of the provided StyledElement."
        );
    }
}
