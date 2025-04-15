namespace Asv.Avalonia;

public sealed class CommandArgMismatchException(Type expected)
    : ArgumentException($"Invalid value type. Argument must be a {expected.FullName}") { }
