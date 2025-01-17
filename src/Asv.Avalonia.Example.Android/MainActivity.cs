using System;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Avalonia;
using Avalonia.Android;

namespace Asv.Avalonia.Example.Android;

[Activity(
    Label = "Asv.Avalonia.Example.Android",
    Theme = "@style/MyTheme.NoActionBar",
    Icon = "@drawable/icon",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.Orientation
        | ConfigChanges.ScreenSize
        | ConfigChanges.UiMode
)]
public class MainActivity : AvaloniaMainActivity<App>
{
    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        using var host = AppHost.Initialize(appHostBuilder =>
        {
            appHostBuilder
                .WithJsonConfiguration("config.json", true, TimeSpan.FromMilliseconds(500))
                .WithAppInfoFrom(typeof(App).Assembly)
                .WithLogMinimumLevel<AppHostConfig>(cfg => cfg.LogMinLevel)
                .WithJsonLogFolder<AppHostConfig>("logs", cfg => cfg.RollingSizeKb)
#if DEBUG
                .WithLogToConsole();
#else
            ;
#endif
        });

        // this is required to use the AndroidHttpClientHandler in main thread
        StrictMode.SetThreadPolicy(new StrictMode.ThreadPolicy.Builder().PermitAll().Build());

        return base.CustomizeAppBuilder(builder).WithInterFont().UseR3();
    }
}
