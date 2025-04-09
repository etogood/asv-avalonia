namespace Asv.Avalonia.Comparers;

public class LogItemDescendingComparer : IComparer<LogItemViewModel>, IEqualityComparer<LogItemViewModel>
{
    public int Compare(LogItemViewModel? x, LogItemViewModel? y)
    {
        if (y != null && x != null)
        {
            return y.Index.CompareTo(x.Index);
        }

        return -1;
    }
    
    public static LogItemDescendingComparer Instance { get; } = new();
    private LogItemDescendingComparer()
    {
    }

    public bool Equals(LogItemViewModel? x, LogItemViewModel? y)
    {
      return Compare(x, y) == 0;
    }

    public int GetHashCode(LogItemViewModel obj)
    {
        return obj.GetHashCode();
    }
}
