using System.Buffers;
using System.Diagnostics;
using Asv.Common;
using Asv.IO;

namespace Asv.Avalonia;

/// <summary>
///
/// [MAGIC] [FORMAT] [HEADER] [METADATA] [DATAxN]
/// </summary>
/// <typeparam name="TData"></typeparam>
/// <typeparam name="TMetadata"></typeparam>
public class RingFile<TData, TMetadata> : IRingFile<TData, TMetadata>
    where TData : ISizedSpanSerializable
    where TMetadata : ISizedSpanSerializable, new()
{
    #region Static

    public bool TryGetFormat(Stream stream, out RingFileFormat? format)
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

    public bool TryGetFormat(string path, out RingFileFormat? format)
    {
        using var fs = new FileStream(path, FileMode.CreateNew, FileAccess.Write);
        return TryGetFormat(fs, out format);
    }

    #endregion

    private readonly int _capacity;
    private readonly RingFileFormat _format;
    private readonly FileStream _fs;
    private readonly object _sync = new();
    private readonly int _metadataStartPosition;
    private readonly int _addressTableStartPosition;
    private readonly int _dataStartPosition;
    private readonly int _totalHeaderSize;

    /*private long head;
    private long tail;
    private bool full;*/

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
            _fs.Position = _metadataStartPosition;
            defaultMetadata.Serialize(_fs);
            Metadata = defaultMetadata;
        }
        else
        {
            _fs = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            if (!TryGetFormat(_fs, out var formatFromFile))
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
        }

        _fs.Position = _metadataStartPosition;
        Metadata = new TMetadata();
        Metadata.Deserialize(_fs);
    }

    public RingFileFormat Format => _format;
    public IRingFileInfo Info { get; }
    public TMetadata Metadata { get; }

    public void EditMetadata(Action<TMetadata> editAction)
    {
        ArgumentNullException.ThrowIfNull(editAction);

        lock (_sync)
        {
            editAction(Metadata);
            _fs.Seek(_metadataStartPosition, SeekOrigin.Begin);
            Metadata.Serialize(_fs);
            _fs.Flush();
        }
    }

    public void Enqueue(TData data)
    {
        /*var size = data.GetByteSize();
        if (size + NodeInfoSize > _capacity)
        {
            throw new InvalidOperationException(
                $"Data size {size} + header size {_totalHeaderSize} exceeds ring file capacity {_capacity}"
            );
        }

        lock (_fs)
        {
            ReadAddressTable(ref head, ref tail, ref full);
            long nextTail = tail;
            long free;
            if (tail >= head)
            {
                free = _capacity - (tail - head) - (full ? 0 : 1);
            }
            else
            {
                free = head - tail - (full ? 0 : 1);
            }

            if (free < size + NodeInfoSize)
            {
                // Буфер полон, нужно продвинуть head
                Span<byte> span = stackalloc byte[4];
                do
                {
                    // Читаем длину первого элемента
                    _fs.Seek(head, SeekOrigin.Begin);
                    _fs.ReadExactly(span);
                    var elemLen = 0;
                    BinSerialize.ReadInt(span, ref elemLen);
                    head = (head + NodeInfoSize + elemLen) % _capacity;
                } while (free < size + NodeInfoSize && head != tail);
            }

            // Запись длины и данных (обработка wrap)
            if (tail + NodeInfoSize + size <= _capacity)
            {
                // Без wrap
                _fs.Seek(_dataStartPosition + tail, SeekOrigin.Begin);
                _fs.Write(BitConverter.GetBytes(size), 0, 4);
                _fs.Write(data, 0, size);
            }
            else
            {
                // Сначала пишем часть, что влезает до конца, потом остальное с начала
                long firstPart = _capacity - tail;
                if (firstPart >= 4)
                {
                    _fs.Seek(_dataStartPosition + tail, SeekOrigin.Begin);
                    _fs.Write(BitConverter.GetBytes(size), 0, 4);
                    if (firstPart > 4)
                    {
                        int dataFirst = (int)(firstPart - 4);
                        _fs.Write(data, 0, dataFirst);
                        _fs.Seek(_dataStartPosition, SeekOrigin.Begin);
                        _fs.Write(data, dataFirst, size - dataFirst);
                    }
                    else
                    {
                        _fs.Seek(_dataStartPosition, SeekOrigin.Begin);
                        _fs.Write(data, 0, size);
                    }
                }
                else
                {
                    // Длина не влезает в конец, пишем в начало
                    _fs.Seek(_dataStartPosition, SeekOrigin.Begin);
                    _fs.Write(BitConverter.GetBytes(size), 0, 4);
                    _fs.Write(data, 0, size);
                }
            }
        }*/
    }

    #region AddressTable

    private const int NodeInfoSize = sizeof(uint) + sizeof(uint);

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
        lock (_sync)
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
