using System.Buffers;
using MemoryPack;

namespace Asv.Avalonia;



[MemoryPackable]
public partial class ChangePropertyCommandState
{
    public string OldValue { get; set; }
    public string NewValue { get; set; }
}

