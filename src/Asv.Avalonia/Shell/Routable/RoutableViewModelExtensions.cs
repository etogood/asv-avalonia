namespace Asv.Avalonia;

public static class RoutableViewModelExtensions
{
    public static IRoutableViewModel GetRoot(this IRoutableViewModel src)
    {
        var root = src;
        while (root.Parent != null)
        {
            root = root.Parent;
        }

        return root;
    }

    public static IEnumerable<IRoutableViewModel> GetAllToRoot(this IRoutableViewModel src)
    {
        var current = src;
        while (current is not null)
        {
            yield return current;
            current = current.Parent;
        }
    }

    public static IEnumerable<IRoutableViewModel> GetAllTo(this IRoutableViewModel src, IRoutableViewModel item)
    {
        var current = src;
        while (current is not null)
        {
            yield return current;
            if (current == item)
            {
                break;
            }

            current = current.Parent;
        }
    }

    public static IEnumerable<IRoutableViewModel> GetAllFromRoot(this IRoutableViewModel src)
    {
        if (src.Parent != null)
        {
            foreach (var ancestor in src.Parent.GetAllFromRoot())
            {
                yield return ancestor;
            }
        }

        yield return src;
    }

    public static IEnumerable<IRoutableViewModel> GetAllFrom(this IRoutableViewModel src, IRoutableViewModel item)
    {
        if (src.Parent != null && src != item)
        {
            foreach (var ancestor in src.Parent.GetAllFrom(item))
            {
                yield return ancestor;
            }
        }

        yield return src;
    }
}