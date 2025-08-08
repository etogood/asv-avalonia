using Asv.Common;

namespace Asv.Avalonia;

public class UnitException(string message) : ValidationException(message) { }
