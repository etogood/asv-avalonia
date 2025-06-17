using Microsoft.Extensions.Logging.Abstractions;
using R3;

namespace Asv.Avalonia;

public class DesignTimeShellViewModel : ShellViewModel
{
    public const string ShellId = "shell.design";
    public static DesignTimeShellViewModel Instance { get; } = new();

    public DesignTimeShellViewModel()
        : base(
            NullContainerHost.Instance,
            NullLoggerFactory.Instance,
            DesignTime.Configuration,
            ShellId
        )
    {
        int cnt = 0;
        ErrorState = ShellErrorState.Error;
        var all = Enum.GetValues<ShellErrorState>().Length;
        TimeProvider.System.CreateTimer(
            _ =>
            {
                cnt++;
                ErrorState = Enum.GetValues<ShellErrorState>()[cnt % all];
#pragma warning disable SA1117
            },
            null,
            TimeSpan.FromSeconds(3),
            TimeSpan.FromSeconds(3)
        );
#pragma warning restore SA1117

        InternalPages.Add(new SettingsPageViewModel());

        var file = new FileMenu(DesignTime.LoggerFactory);
        MainMenu.Add(file);
        MainMenu.Add(new MenuItem("open", "Open", DesignTime.LoggerFactory, file.Id.Id));
        MainMenu.Add(new EditMenu(DesignTime.LoggerFactory));
    }

    public override INavigationService Navigation => DesignTime.Navigation;
}
