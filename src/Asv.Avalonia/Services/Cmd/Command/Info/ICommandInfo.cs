using Material.Icons;

namespace Asv.Avalonia;

public interface ICommandInfo : IExportable
{
    string Id { get; init; }
    string Name { get; init; }
    string Description { get; init; }
    MaterialIconKind Icon { get; init; }
    HotKeyInfo? DefaultHotKey { get; init; }
}
