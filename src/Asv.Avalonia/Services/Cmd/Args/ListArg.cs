using System.Collections;
using Asv.IO;

namespace Asv.Avalonia;

public class ListArg : CommandArg, IList<CommandArg>
{
    public static CommandArg Create() => new ListArg();

    private readonly List<CommandArg> _items;

    public ListArg() => _items = [];

    public ListArg(int capacity) => _items = new List<CommandArg>(capacity);

    public override Id TypeId => Id.Array;

    protected override void InternalDeserialize(ref ReadOnlySpan<byte> buffer)
    {
        var size = BinSerialize.ReadPackedUnsignedInteger(ref buffer);
        Clear();
        for (var i = 0; i < size; i++)
        {
            var element = Create(ref buffer);
            element.Deserialize(ref buffer);
            Add(element);
        }
    }

    protected override void InternalSerialize(ref Span<byte> buffer)
    {
        var size = Count; // Assuming Count is the number of elements in the list
        BinSerialize.WritePackedUnsignedInteger(ref buffer, (uint)size);
        foreach (var item in this)
        {
            item.Serialize(ref buffer);
        }
    }

    protected override int InternalGetByteSize() =>
        BinSerialize.GetSizeForPackedUnsignedInteger((uint)Count)
        + this.Sum(item => item.GetByteSize());

    public IEnumerator<CommandArg> GetEnumerator() => _items.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Add(CommandArg item) => _items.Add(item);

    public void Clear() => _items.Clear();

    public bool Contains(CommandArg item) => _items.Contains(item);

    public void CopyTo(CommandArg[] array, int arrayIndex) => _items.CopyTo(array, arrayIndex);

    public bool Remove(CommandArg item) => _items.Remove(item);

    public int Count => _items.Count;
    public bool IsReadOnly => false;

    public int IndexOf(CommandArg item) => _items.IndexOf(item);

    public void Insert(int index, CommandArg item) => _items.Insert(index, item);

    public void RemoveAt(int index) => _items.RemoveAt(index);

    public CommandArg this[int index]
    {
        get => _items[index];
        set => _items[index] = value;
    }
}
