using System.Diagnostics;
using System.IO.Hashing;
using Asv.Common;
using Asv.IO;

namespace Asv.Avalonia;

public sealed class RingFileFormat : ISizedSpanSerializable, IEquatable<RingFileFormat>
{
    public const int MaxSize =
        256 /*0x0100*/
    ;
    private const string Magic = "ASV_RING_FILE";
    private readonly int _maxMetadataSize;

    internal RingFileFormat(ReadOnlySpan<byte> buffer)
    {
        if (buffer.Length != MaxSize)
        {
            throw new InvalidOperationException($"Buffer size must be exactly {MaxSize} bytes.");
        }

        var crcSpan = buffer[^sizeof(uint)..];
        var dataSpan = buffer[..^sizeof(uint)];
        Debug.Assert(
            dataSpan.Length == MaxSize - sizeof(uint),
            "Data span must be exactly MaxSize - 4 bytes long."
        );
        Debug.Assert(crcSpan.Length == sizeof(uint), "CRC span must be exactly 4 bytes long.");
        var hash = Crc32.HashToUInt32(dataSpan);
        if (hash != BitConverter.ToUInt32(crcSpan))
        {
            throw new InvalidDataException(
                $"CRC32 hash mismatch. Expected {BitConverter.ToUInt32(crcSpan):X8}, but got {hash:X8}"
            );
        }

        var magic = BinSerialize.ReadString(ref buffer);
        if (magic != Magic)
        {
            throw new InvalidDataException(
                $"Invalid magic value. Expected '{Magic}', but got '{magic}'"
            );
        }

        Type = BinSerialize.ReadString(ref buffer);
        Version = SemVersion.Parse(BinSerialize.ReadString(ref buffer));
        BinSerialize.ReadInt(ref buffer, ref _maxMetadataSize);
    }

    public RingFileFormat(string type, SemVersion version, int maxMetadataSize)
    {
        Type = type ?? throw new ArgumentNullException(nameof(type));
        Version = version ?? throw new ArgumentNullException(nameof(version));
        _maxMetadataSize = maxMetadataSize;
    }

    public string Type { get; }
    public SemVersion Version { get; }
    public int MaxMetadataSize => _maxMetadataSize;

    public void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        // This method is not used in this implementation, as the constructor handles deserialization.
    }

    public void Serialize(ref Span<byte> buffer)
    {
        if (buffer.Length != MaxSize)
        {
            throw new InvalidOperationException($"Buffer size must be exactly {MaxSize} bytes.");
        }

        var origin = buffer;
        var crcSpan = buffer[^sizeof(uint)..];
        Debug.Assert(crcSpan.Length == sizeof(uint), "CRC span must be exactly 4 bytes long.");
        BinSerialize.WriteString(ref buffer, Magic);
        BinSerialize.WriteString(ref buffer, Type);
        BinSerialize.WriteString(ref buffer, Version.ToString());
        BinSerialize.WriteInt(ref buffer, MaxMetadataSize);
        if (buffer.Length < sizeof(int))
        {
            throw new InvalidOperationException(
                $"Buffer size must be at least {sizeof(int)} bytes to hold the CRC32 hash."
            );
        }

        buffer.Clear();
        var hashSize = Crc32.Hash(origin, crcSpan);
        Debug.Assert(hashSize == sizeof(uint), "CRC32 hash size must be exactly 4 bytes.");
    }

    public int GetByteSize() => MaxSize;

    public bool Equals(RingFileFormat? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return _maxMetadataSize == other._maxMetadataSize
            && string.Equals(Type, other.Type, StringComparison.InvariantCultureIgnoreCase)
            && Version.Equals(other.Version);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null)
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj.GetType() != GetType())
        {
            return false;
        }

        return Equals((RingFileFormat)obj);
    }

    public override int GetHashCode()
    {
        var hashCode = default(HashCode);
        hashCode.Add(_maxMetadataSize);
        hashCode.Add(Type, StringComparer.InvariantCultureIgnoreCase);
        hashCode.Add(Version);
        return hashCode.ToHashCode();
    }

    public static bool operator ==(RingFileFormat? left, RingFileFormat? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(RingFileFormat? left, RingFileFormat? right)
    {
        return !Equals(left, right);
    }

    public override string ToString()
    {
        return $"{Magic} {Type} {Version} MaxMetadataSize:{_maxMetadataSize}";
    }
}
