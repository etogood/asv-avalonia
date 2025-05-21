namespace Asv.Avalonia;

public sealed class HotkeyAlreadyExists(string hotKey)
    : Exception($"Hotkey {hotKey} already exists") { }
