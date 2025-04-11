namespace Asv.Avalonia;

public class ListCommandArg<T> : ICommandArg
{
    public ListCommandArg(IList<T>? items = null)
    {
        Items = items;
    }

    public IList<T>? Items { get; }
}
