using System.Buffers;
using System.Diagnostics;
using Asv.IO;

namespace Asv.Avalonia;

/// <summary>
///
/// [MAGIC] [FORMAT] [METADATA] [ADDRESS] [DATAxN]
/// </summary>
/// <typeparam name="TData"></typeparam>
/// <typeparam name="TMetadata"></typeparam>
public class RingFile<TData, TMetadata> : IRingFile<TData, TMetadata>
    where TData : ISizedSpanSerializable
    where TMetadata : ISizedSpanSerializable, new()
{
    #region Static

    public static bool TryReadFormat(Stream stream, out RingFileFormat? format)
    {
        var buff = ArrayPool<byte>.Shared.Rent(RingFileFormat.MaxSize);
        try
        {
            var span = new Span<byte>(buff, 0, RingFileFormat.MaxSize);
            stream.Seek(0, SeekOrigin.Begin);
            if (stream.Read(span) != RingFileFormat.MaxSize)
            {
                format = null;
                return false;
            }

            var readSpan = new ReadOnlySpan<byte>(buff, 0, RingFileFormat.MaxSize);
            format = new RingFileFormat(readSpan);
            return true;
        }
        catch (Exception ex)
        {
            format = null;
            return false;
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buff);
        }
    }

    public static bool TryReadFormat(string path, out RingFileFormat? format)
    {
        using var fs = new FileStream(path, FileMode.Open, FileAccess.Read);
        return TryReadFormat(fs, out format);
    }

    #endregion

    private readonly object _sync = new();
    private readonly int _capacity;
    private readonly RingFileFormat _format;
    private readonly FileStream _fs;
    private readonly long _metadataStartPosition;
    private readonly long _addressTableStartPosition;
    private readonly long _dataStartPosition;
    private readonly long _totalHeaderSize;

    private long head;
    private long tail;
    private bool full;

    public RingFile(string path, int capacity, RingFileFormat format, TMetadata defaultMetadata)
    {
        _capacity = capacity;
        _format = format;
        _metadataStartPosition = RingFileFormat.MaxSize;
        _addressTableStartPosition = _metadataStartPosition + format.MaxMetadataSize;
        _dataStartPosition = _addressTableStartPosition + AddressTableSize;
        _totalHeaderSize = _dataStartPosition;

        if (!File.Exists(path))
        {
            _fs = new FileStream(path, FileMode.CreateNew, FileAccess.Write);
            _format.Serialize(_fs);
            WriteMetadata(defaultMetadata);
            head = 0;
            tail = 0;
            full = false;
            WriteAddressTable(head, tail, full);
        }
        else
        {
            _fs = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            if (!TryReadFormat(_fs, out var formatFromFile))
            {
                throw new InvalidOperationException($"Cannot read format from file {path}");
            }

            Debug.Assert(formatFromFile != null, nameof(formatFromFile) + " != null");

            if (!formatFromFile.Equals(_format))
            {
                throw new InvalidOperationException(
                    $"File format {path} is not compatible with expected format {_format}"
                );
            }

            ReadAddressTable(ref head, ref tail, ref full);
        }
    }

    public RingFileFormat Format => _format;

    public TMetadata ReadMetadata()
    {
        _fs.Position = _metadataStartPosition;
        var metadata = new TMetadata();
        metadata.Deserialize(_fs);
        return metadata;
    }

    public void WriteMetadata(TMetadata metadata)
    {
        ArgumentNullException.ThrowIfNull(metadata);
        _fs.Seek(_metadataStartPosition, SeekOrigin.Begin);
        metadata.Serialize(_fs);
        _fs.Flush();
    }

    public void EditMetadata(Action<TMetadata> editAction)
    {
        ArgumentNullException.ThrowIfNull(editAction);
        var metadata = ReadMetadata();
        editAction(metadata);
        WriteMetadata(metadata);
    }

    private long FreeSpace
    {
        get
        {
            if (tail >= head)
            {
                return _capacity - (tail - head);
            }

            return head - tail;
        }
    }

    private void RemoveOldestElement()
    {
        if (head == tail && !full)
        {
            throw new InvalidOperationException("Cannot remove element from an empty ring file.");
        }

        Span<byte> span = stackalloc byte[4];
        _fs.Seek(_dataStartPosition + head, SeekOrigin.Begin);
        _fs.ReadExactly(span);
        var elemLen = 0U;
        BinSerialize.ReadUInt(span, ref elemLen);
        head = (head + NodeMetadataSize + elemLen) % _capacity;

        if (head == tail)
        {
            full = false; // Ring is now empty
        }

        WriteAddressTable(head, tail, full);
    }

    public void Push(TData data)
    {
        var dataSize = data.GetByteSize();
        var needSize = dataSize + NodeMetadataSize;
        if (needSize > _capacity)
        {
            throw new InvalidOperationException(
                $"Data size {needSize} + header size {_totalHeaderSize} exceeds ring file capacity {_capacity}"
            );
        }

        while (FreeSpace < needSize)
        {
            RemoveOldestElement();
        }

        var dataArray = ArrayPool<byte>.Shared.Rent(needSize);
        try
        {
            var dataArraySpan = new Span<byte>(dataArray, 0, needSize);
            BinSerialize.WriteUInt(ref dataArraySpan, (uint)dataSize);
            data.Serialize(ref dataArraySpan);
            BinSerialize.WriteUInt(ref dataArraySpan, (uint)dataSize);
            Debug.Assert(dataArraySpan.Length == 0, "dataArraySpan should be fully written");
            if (
                tail + needSize
                <= _capacity - 4 /* space for size */
            )
            {
                // enough space in the tail, write directly
                _fs.Seek(_dataStartPosition + tail, SeekOrigin.Begin);
                _fs.Write(dataArray, 0, needSize);
            }
            else
            {
                // not enough space in the tail, need to wrap
                var firstSpan = new ReadOnlySpan<byte>(dataArray, 0, (int)(_capacity - tail));
                var secondSpan = new ReadOnlySpan<byte>(
                    dataArray,
                    (int)(_capacity - tail),
                    needSize - (int)(_capacity - tail)
                );
                _fs.Seek(_dataStartPosition + tail, SeekOrigin.Begin);
                _fs.Write(firstSpan);
                _fs.Seek(_dataStartPosition, SeekOrigin.Begin);
                _fs.Write(secondSpan);
            }
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(dataArray);
        }

        tail = (tail + needSize) % _capacity;
        full = tail == head;
        WriteAddressTable(head, tail, full);
    }

    public TData Pop()
    {
        throw new NotImplementedException();
    }

    #region AddressTable

    private const int NodeMetadataSize = sizeof(uint) + sizeof(uint);

    private const int AddressTableSize = sizeof(long) + sizeof(long) + sizeof(bool);

    private void ReadAddressTable(ref long head, ref long tail, ref bool full)
    {
        _fs.Seek(_addressTableStartPosition, SeekOrigin.Begin);
        var buff = ArrayPool<byte>.Shared.Rent(AddressTableSize);
        try
        {
            var span = new Span<byte>(buff, 0, AddressTableSize);
            if (_fs.Read(span) != AddressTableSize)
            {
                throw new InvalidOperationException("Cannot read address table from file");
            }

            var readSpan = new ReadOnlySpan<byte>(buff, 0, AddressTableSize);
            BinSerialize.ReadLong(readSpan, ref head);
            BinSerialize.ReadLong(readSpan, ref tail);
            BinSerialize.ReadBool(readSpan, ref full);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buff);
        }
    }

    private void WriteAddressTable(in long head, in long tail, in bool full)
    {
        _fs.Seek(_addressTableStartPosition, SeekOrigin.Begin);
        var buff = ArrayPool<byte>.Shared.Rent(AddressTableSize);
        try
        {
            var span = new Span<byte>(buff, 0, AddressTableSize);
            BinSerialize.WriteLong(ref span, in head);
            BinSerialize.WriteLong(ref span, in tail);
            BinSerialize.WriteBool(ref span, full);
            _fs.Write(buff, 0, AddressTableSize);
            _fs.Flush();
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buff);
        }
    }

    #endregion

    #region Dispose

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            lock (_sync)
            {
                _fs.Dispose();
            }
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    #endregion
}
