namespace Asv.Avalonia;

public class StatusItemComparer : IComparer<IStatusItem>
{
    public static IComparer<IStatusItem> Instance { get; } = new StatusItemComparer();

    public int Compare(IStatusItem? x, IStatusItem? y)
    {
        if (ReferenceEquals(x, y))
        {
            return 0;
        }

        if (y is null)
        {
            return 1;
        }

        if (x is null)
        {
            return -1;
        }

        return x.Order.CompareTo(y.Order);
    }
}
