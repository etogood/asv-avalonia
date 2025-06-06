using Asv.Avalonia.Routable;

namespace Asv.Avalonia.Example.Api;

public interface IWellKnownCommands { }

public class ContextCommand<TContext>
{
    public interface WithArg<TArgument>
    {
        ValueTask Execute(TContext context, TArgument argument);
    }
}

public class Command1
    : ContextCommand<IRoutable>.WithArg<StringArg>,
        ContextCommand<IRoutable>.WithArg<DoubleArg>
{
    public ValueTask Execute(IRoutable context, StringArg argument)
    {
        throw new NotImplementedException();
    }

    public ValueTask Execute(IRoutable context, DoubleArg argument)
    {
        throw new NotImplementedException();
    }
}

public class ApiModule : IExportInfo
{
    public static readonly ApiModule Instance = new();
    public string ModuleName => "Asv.Avalonia.Example.Api";
}
