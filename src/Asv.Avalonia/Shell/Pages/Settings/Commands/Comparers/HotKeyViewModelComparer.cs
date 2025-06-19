namespace Asv.Avalonia;

public sealed class HotKeyViewModelComparer : IComparer<HotKeyViewModel>
{
    public static readonly HotKeyViewModelComparer Instance = new();

    private HotKeyViewModelComparer() { }

    public int Compare(HotKeyViewModel? x, HotKeyViewModel? y)
    {
        if (ReferenceEquals(x, y))
        {
            return 0;
        }

        if (x is null)
        {
            return 1;
        }

        if (y is null)
        {
            return -1;
        }

        return string.CompareOrdinal(x.Id.Id, y.Id.Id);
    }
}
