using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using Asv.IO;

namespace Asv.Avalonia;

public class DictArg : CommandArg, IDictionary<string, CommandArg>
{
    private readonly Dictionary<string, CommandArg> _dictionary = new(
        StringComparer.InvariantCultureIgnoreCase
    );

    public static CommandArg Create() => new DictArg();

    public override Id TypeId => Id.Dict;

    protected override void InternalDeserialize(ref ReadOnlySpan<byte> buffer)
    {
        var size = BinSerialize.ReadPackedUnsignedInteger(ref buffer);
        Clear();
        for (var i = 0; i < size; i++)
        {
            var key = BinSerialize.ReadString(ref buffer);
            var element = Create(ref buffer);
            element.Deserialize(ref buffer);
            Add(key, element);
        }
    }

    protected override void InternalSerialize(ref Span<byte> buffer)
    {
        var size = _dictionary.Count;
        BinSerialize.WritePackedUnsignedInteger(ref buffer, (uint)size);
        foreach (var kvp in _dictionary)
        {
            BinSerialize.WriteString(ref buffer, kvp.Key);
            kvp.Value.Serialize(ref buffer);
        }
    }

    protected override int InternalGetByteSize() =>
        BinSerialize.GetSizeForPackedUnsignedInteger((uint)_dictionary.Count)
        + _dictionary.Sum(kvp => BinSerialize.GetSizeForString(kvp.Key) + kvp.Value.GetByteSize());

    public IEnumerator<KeyValuePair<string, CommandArg>> GetEnumerator() =>
        _dictionary.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Add(KeyValuePair<string, CommandArg> item) => _dictionary.Add(item.Key, item.Value);

    public void Clear() => _dictionary.Clear();

    public bool Contains(KeyValuePair<string, CommandArg> item) => _dictionary.Contains(item);

    public void CopyTo(KeyValuePair<string, CommandArg>[] array, int arrayIndex) =>
        ((ICollection<KeyValuePair<string, CommandArg>>)_dictionary).CopyTo(array, arrayIndex);

    public bool Remove(KeyValuePair<string, CommandArg> item) => _dictionary.Remove(item.Key);

    public int Count => _dictionary.Count;
    public bool IsReadOnly => false;

    public void Add(string key, CommandArg value) => _dictionary.Add(key, value);

    public bool ContainsKey(string key) => _dictionary.ContainsKey(key);

    public bool Remove(string key) => _dictionary.Remove(key);

    public bool TryGetValue(string key, [MaybeNullWhen(false)] out CommandArg value) =>
        _dictionary.TryGetValue(key, out value);

    public CommandArg this[string key]
    {
        get => _dictionary[key];
        set => _dictionary[key] = value;
    }

    public ICollection<string> Keys => _dictionary.Keys;
    public ICollection<CommandArg> Values => _dictionary.Values;
}
