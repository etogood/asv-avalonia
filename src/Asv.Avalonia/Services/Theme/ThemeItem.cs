using Avalonia.Styling;

namespace Asv.Avalonia;

public class ThemeItem(string id, string name, ThemeVariant theme) : IThemeInfo
{
    public string Id { get; } = id;
    public string Name { get; } = name;
    public ThemeVariant Theme { get; } = theme;
}