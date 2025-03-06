using System;
using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Asv.Avalonia;

/// <summary>
/// Represents a unique identifier for navigation, consisting of a type identifier and optional arguments.
/// </summary>
public struct NavigationArgs
    : IEquatable<NavigationArgs>,
        IComparable<NavigationArgs>,
        IList<KeyValuePair<string, string?>>,
        IReadOnlyList<KeyValuePair<string, string?>>
{
    public static readonly NavigationArgs Empty = new();

    private const int OverflowAdditionalCapacity = 8;

    [InlineArray(OverflowAdditionalCapacity)]
    private struct InlineTags
    {
        public const int Length = OverflowAdditionalCapacity;
        private KeyValuePair<string, string?> _first;
    }

    private InlineTags _args;
    private int _argsCount;
    private KeyValuePair<string, string?>[]? _overflowTags;

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationArgs"/> struct with a type identifier and optional arguments.
    /// </summary>
    /// <param name="argList">The optional arguments associated with the navigation identifier.</param>
    public NavigationArgs(ReadOnlySpan<KeyValuePair<string, string?>> argList = default)
    {
        _argsCount = argList.Length;
        scoped Span<KeyValuePair<string, string?>> tags =
            _argsCount <= InlineTags.Length
                ? _args
                : _overflowTags = new KeyValuePair<string, string?>[
                    _argsCount + OverflowAdditionalCapacity
                ];

        argList.CopyTo(tags);
    }

    public readonly int Count => _argsCount;
    public readonly bool IsReadOnly => false;

    public void Insert(int index, KeyValuePair<string, string?> item)
    {
        if (index == _argsCount)
        {
            Add(item);
            return;
        }

        ArgumentOutOfRangeException.ThrowIfGreaterThan(
            (uint)index,
            (uint)_argsCount,
            nameof(index)
        );

        if (_argsCount == InlineTags.Length && _overflowTags is null)
        {
            _overflowTags = new KeyValuePair<string, string?>[
                InlineTags.Length + OverflowAdditionalCapacity
            ];
            ((ReadOnlySpan<KeyValuePair<string, string?>>)_args).CopyTo(_overflowTags);
        }

        if (_overflowTags is not null)
        {
            if (_argsCount == _overflowTags.Length)
            {
                Array.Resize(ref _overflowTags, _argsCount + OverflowAdditionalCapacity);
            }

            _overflowTags.AsSpan(index, _argsCount - index).CopyTo(_overflowTags.AsSpan(index + 1));
            _overflowTags[index] = item;
        }
        else
        {
            Span<KeyValuePair<string, string?>> tags = _args;
            tags.Slice(index, _argsCount - index).CopyTo(tags[(index + 1)..]);
            tags[index] = item;
        }

        _argsCount++;
    }

    public void RemoveAt(int index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(
            (uint)index,
            (uint)_argsCount,
            nameof(index)
        );

        Span<KeyValuePair<string, string?>> tags = _overflowTags is not null
            ? _overflowTags
            : _args;
        tags.Slice(index + 1, _argsCount - index - 1).CopyTo(tags[index..]);
        _argsCount--;
    }

    public KeyValuePair<string, string?> this[int index]
    {
        readonly get
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(
                (uint)index,
                (uint)_argsCount,
                nameof(index)
            );

            return _overflowTags is null ? _args[index] : _overflowTags[index];
        }
        set
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(
                (uint)index,
                (uint)_argsCount,
                nameof(index)
            );

            if (_overflowTags is null)
            {
                _args[index] = value;
            }
            else
            {
                _overflowTags[index] = value;
            }
        }
    }

    public string? this[string name]
    {
        readonly get
        {
            if (_overflowTags is null)
            {
                for (var i = 0; i < _argsCount; i++)
                {
                    if (this[i].Key == name)
                    {
                        return this[i].Value;
                    }
                }
            }
            else
            {
                for (var i = 0; i < _argsCount; i++)
                {
                    if (_overflowTags[i].Key == name)
                    {
                        return _overflowTags[i].Value;
                    }
                }
            }

            return null;
        }
        set
        {
            if (_overflowTags is null)
            {
                for (var i = 0; i < _argsCount; i++)
                {
                    if (this[i].Key == name)
                    {
                        this[i] = new KeyValuePair<string, string?>(name, value);
                        return;
                    }
                }
            }
            else
            {
                for (var i = 0; i < _argsCount; i++)
                {
                    if (_overflowTags[i].Key == name)
                    {
                        _overflowTags[i] = new KeyValuePair<string, string?>(name, value);
                        return;
                    }
                }
            }

            Add(name, value);
        }
    }

    public void Add(string key, string? value) =>
        Add(new KeyValuePair<string, string?>(key, value));

    public void Add(KeyValuePair<string, string?> tag)
    {
        int count = _argsCount;
        if (_overflowTags is null && (uint)count < InlineTags.Length)
        {
            _args[count] = tag;
            _argsCount++;
        }
        else
        {
            AddToOverflow(tag);
        }
    }

    public void Clear() => _argsCount = 0;

    public bool Contains(KeyValuePair<string, string?> item)
    {
        return IndexOf(item) >= 0;
    }

    public readonly void CopyTo(KeyValuePair<string, string?>[] array, int arrayIndex)
    {
        ArgumentNullException.ThrowIfNull(array);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(
            (uint)arrayIndex,
            (uint)array.Length,
            nameof(arrayIndex)
        );

        CopyTo(array.AsSpan(arrayIndex));
    }

    public readonly void CopyTo(Span<KeyValuePair<string, string?>> tags)
    {
        if (tags.Length < _argsCount)
        {
            throw new ArgumentException("Destination span is not large enough to hold the tags.");
        }

        Args.CopyTo(tags);
    }

    private void AddToOverflow(KeyValuePair<string, string?> tag)
    {
        Debug.Assert(_overflowTags is not null || _argsCount == InlineTags.Length);

        if (_overflowTags is null)
        {
            _overflowTags = new KeyValuePair<string, string?>[
                InlineTags.Length + OverflowAdditionalCapacity
            ];
            ((ReadOnlySpan<KeyValuePair<string, string?>>)_args).CopyTo(_overflowTags);
        }
        else if (_argsCount == _overflowTags.Length)
        {
            Array.Resize(ref _overflowTags, _argsCount + OverflowAdditionalCapacity);
        }

        _overflowTags[_argsCount] = tag;
        _argsCount++;
    }

    [UnscopedRef]
    internal readonly ReadOnlySpan<KeyValuePair<string, string?>> Args =>
        _overflowTags is not null
            ? _overflowTags.AsSpan(0, _argsCount)
            : ((ReadOnlySpan<KeyValuePair<string, string?>>)_args).Slice(0, _argsCount);

    public struct Enumerator : IEnumerator<KeyValuePair<string, string?>>
    {
        private readonly NavigationArgs _tagList;
        private int _index;

        internal Enumerator(in NavigationArgs tagList)
        {
            _index = -1;
            _tagList = tagList;
        }

        public KeyValuePair<string, string?> Current => _tagList[_index];

        object IEnumerator.Current => _tagList[_index];

        public void Dispose()
        {
            _index = _tagList.Count;
        }

        public bool MoveNext()
        {
            _index++;
            return _index < _tagList.Count;
        }

        public void Reset() => _index = -1;
    }

    public bool Remove(KeyValuePair<string, string?> item)
    {
        int index = IndexOf(item);
        if (index >= 0)
        {
            RemoveAt(index);
            return true;
        }

        return false;
    }

    public readonly IEnumerator<KeyValuePair<string, string?>> GetEnumerator() =>
        new Enumerator(this);

    readonly IEnumerator IEnumerable.GetEnumerator() => new Enumerator(in this);

    public readonly int IndexOf(KeyValuePair<string, string?> item)
    {
        ReadOnlySpan<KeyValuePair<string, string?>> tags = _overflowTags is not null
            ? _overflowTags
            : _args;

        tags = tags.Slice(0, _argsCount);

        if (item.Value is not null)
        {
            for (int i = 0; i < tags.Length; i++)
            {
                if (item.Key == tags[i].Key && item.Value.Equals(tags[i].Value))
                {
                    return i;
                }
            }
        }
        else
        {
            for (int i = 0; i < tags.Length; i++)
            {
                if (item.Key == tags[i].Key && tags[i].Value is null)
                {
                    return i;
                }
            }
        }

        return -1;
    }

    #region IEquatable

    /// <summary>
    /// Compares two <see cref="NavigationArgs"/> instances for equality using case-insensitive comparison.
    /// Equality is determined by comparing <see cref="Id"/> and the collection of arguments.
    /// </summary>
    /// <param name="left">The first <see cref="NavigationArgs"/> to compare.</param>
    /// <param name="right">The second <see cref="NavigationArgs"/> to compare.</param>
    /// <returns><c>true</c> if the instances are equal; otherwise, <c>false</c>.</returns>
    public static bool operator ==(NavigationArgs left, NavigationArgs right) => left.Equals(right);

    /// <summary>
    /// Compares two <see cref="NavigationArgs"/> instances for inequality using case-insensitive comparison.
    /// </summary>
    /// <param name="left">The first <see cref="NavigationArgs"/> to compare.</param>
    /// <param name="right">The second <see cref="NavigationArgs"/> to compare.</param>
    /// <returns><c>true</c> if the instances are not equal; otherwise, <c>false</c>.</returns>
    public static bool operator !=(NavigationArgs left, NavigationArgs right) =>
        !left.Equals(right);

    /// <summary>
    /// Determines whether the current <see cref="NavigationArgs"/> is equal to another <see cref="NavigationArgs"/> instance,
    /// ignoring case differences in <see cref="Id"/> and argument keys/values.
    /// </summary>
    /// <param name="other">The <see cref="NavigationArgs"/> to compare with the current instance.</param>
    /// <returns><c>true</c> if the instances are equal; otherwise, <c>false</c>.</returns>
    public bool Equals(NavigationArgs other)
    {
        if (_argsCount != other._argsCount)
        {
            return false;
        }

        var thisArgs = Args;
        var otherArgs = other.Args;

        for (int i = 0; i < _argsCount; i++)
        {
            var thisPair = thisArgs[i];
            var otherPair = otherArgs[i];
            if (
                !string.Equals(
                    thisPair.Key,
                    otherPair.Key,
                    StringComparison.InvariantCultureIgnoreCase
                )
                || !string.Equals(
                    thisPair.Value,
                    otherPair.Value,
                    StringComparison.InvariantCultureIgnoreCase
                )
            )
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current <see cref="NavigationArgs"/> instance.
    /// </summary>
    /// <param name="obj">The object to compare with the current instance.</param>
    /// <returns><c>true</c> if the object is a <see cref="NavigationArgs"/> and equal to the current instance; otherwise, <c>false</c>.</returns>
    public override bool Equals(object? obj)
    {
        return obj is NavigationArgs other && Equals(other);
    }

    /// <summary>
    /// Returns a hash code for the current <see cref="NavigationArgs"/> instance, computed using case-insensitive comparison
    /// of <see cref="Id"/> and all arguments.
    /// </summary>
    /// <returns>A hash code for this instance.</returns>
    public override int GetHashCode()
    {
        var hash = default(HashCode);

        var args = Args;
        for (int i = 0; i < _argsCount; i++)
        {
            hash.Add(args[i].Key, StringComparer.InvariantCultureIgnoreCase);
            hash.Add(args[i].Value, StringComparer.InvariantCultureIgnoreCase);
        }

        return hash.ToHashCode();
    }

    #endregion

    #region IComparable

    /// <summary>
    /// Compares the current instance with another <see cref="NavigationArgs"/> instance and returns an integer that indicates
    /// whether the current instance precedes, follows, or occurs in the same position in the sort order as the other instance.
    /// Comparison is performed first by <see cref="Id"/>, then by argument count, and then by argument key-value pairs,
    /// ignoring case differences.
    /// </summary>
    /// <param name="other">The <see cref="NavigationArgs"/> to compare with the current instance.</param>
    /// <returns>
    /// A value less than zero if this instance precedes <paramref name="other"/>,
    /// zero if this instance equals <paramref name="other"/>,
    /// or a value greater than zero if this instance follows <paramref name="other"/> in the sort order.
    /// </returns>
    public int CompareTo(NavigationArgs other)
    {
        // Сравниваем количество аргументов
        int countComparison = _argsCount.CompareTo(other._argsCount);
        if (countComparison != 0)
        {
            return countComparison;
        }

        // Если количество равно, сравниваем сами аргументы
        var thisArgs = Args;
        var otherArgs = other.Args;

        for (int i = 0; i < _argsCount; i++)
        {
            var thisPair = thisArgs[i];
            var otherPair = otherArgs[i];

            // Сравниваем ключи
            int keyComparison = StringComparer.InvariantCultureIgnoreCase.Compare(
                thisPair.Key,
                otherPair.Key
            );
            if (keyComparison != 0)
            {
                return keyComparison;
            }

            // Если ключи равны, сравниваем значения
            int valueComparison = StringComparer.InvariantCultureIgnoreCase.Compare(
                thisPair.Value,
                otherPair.Value
            );
            if (valueComparison != 0)
            {
                return valueComparison;
            }
        }

        return 0; // Полное равенство
    }

    #endregion

    /// <summary>
    /// Returns a string representation of the current <see cref="NavigationArgs"/> instance.
    /// </summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString() =>
        string.Join(",", Args.ToArray().Select(kvp => $"{kvp.Key}={kvp.Value}"));
}
