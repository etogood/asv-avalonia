namespace Asv.Avalonia;

public static class RoutableViewModelMixin
{
    public static IRoutable GetRoot(this IRoutable src)
    {
        var root = src;
        while (root.NavigationParent != null)
        {
            root = root.NavigationParent;
        }

        return root;
    }

    public static IEnumerable<IRoutable> GetAllToRoot(this IRoutable src)
    {
        var current = src;
        while (current is not null)
        {
            yield return current;
            current = current.NavigationParent;
        }
    }

    public static IEnumerable<IRoutable> GetAllTo(this IRoutable src, IRoutable item)
    {
        var current = src;
        while (current is not null)
        {
            yield return current;
            if (current == item)
            {
                break;
            }

            current = current.NavigationParent;
        }
    }

    public static IEnumerable<IRoutable> GetAllFromRoot(this IRoutable src)
    {
        if (src.NavigationParent != null)
        {
            foreach (var ancestor in src.NavigationParent.GetAllFromRoot())
            {
                yield return ancestor;
            }
        }

        yield return src;
    }

    public static IEnumerable<IRoutable> GetAllFrom(this IRoutable src, IRoutable item)
    {
        if (src.NavigationParent != null && src != item)
        {
            foreach (var ancestor in src.NavigationParent.GetAllFrom(item))
            {
                yield return ancestor;
            }
        }

        yield return src;
    }
}