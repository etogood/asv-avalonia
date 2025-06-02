namespace Asv.Avalonia;

public sealed class CommandArgMismatchException : ArgumentException
{
    public CommandArgMismatchException(Type expected)
        : base($"Invalid value type. Argument must be a {expected.FullName}") { }

    public CommandArgMismatchException(Type expected, string? paramName)
        : base(
            $"Invalid value type. Argument '{paramName}' must be a {expected.FullName}",
            paramName
        ) { }
}
