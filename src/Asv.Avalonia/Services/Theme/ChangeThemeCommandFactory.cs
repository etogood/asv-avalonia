using System.Composition;

namespace Asv.Avalonia;

[Export(typeof(ICommandFactory))]
[Shared]
public class ChangeThemeCommandFactory : ICommandFactory
{
    private readonly IThemeService _svc;
    private readonly IShellHost _host;

    [ImportingConstructor]
    public ChangeThemeCommandFactory(IThemeService svc, IShellHost host)
    {
        ArgumentNullException.ThrowIfNull(svc);
        _svc = svc;
        _host = host;
    }

    public string CommandId => ChangeThemeCommand.CommandId;
    public string Name => "Change theme";
    public string Description => "Change application theme";
    public string Icon { get; } = string.Empty;
    public int Order => 0;

    public ICommandBase Create()
    {
        return new ChangeThemeCommand(_svc);
    }
}

public class ChangeThemeCommand(IThemeService svc) : ICommandBase
{
    public const string CommandId = "theme.change";
    public IMemento Save()
    {
        throw new NotImplementedException();
    }

    public void Restore(IMemento state)
    {
        throw new NotImplementedException();
    }

    public string Id => CommandId;

    public ValueTask Execute(object? context, IMemento? parameter = null, CancellationToken cancel = default)
    {
        if (parameter is Memento<string> memento)
        {
            var theme = svc.Themes.FirstOrDefault(x => x.Id == memento.Value);
            if (theme != null)
            {
                svc.CurrentTheme.OnNext(theme);
            }
        }

        return ValueTask.CompletedTask;
    }
}