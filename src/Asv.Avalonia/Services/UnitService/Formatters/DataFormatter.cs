namespace Asv.Avalonia;

public static class DataFormatter
{
    public static IDataFormatter ByteRate { get; } = new ByteRateFormatter();
}
