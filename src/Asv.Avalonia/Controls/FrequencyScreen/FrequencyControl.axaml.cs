using Avalonia;
using Avalonia.Controls.Primitives;

namespace Asv.Avalonia;

public class FrequencyControl : TemplatedControl
{
    public static readonly StyledProperty<string?> ValueProperty = AvaloniaProperty.Register<
        FrequencyControl,
        string?
    >(nameof(Value));

    public string? Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }
}
