using System;
using UIKit;

namespace Asv.Avalonia.Example.iOS;

public class Application
{
    // This is the main entry point of the application.
    static void Main(string[] args)
    {
        using var host = AppHost.Initialize(builder =>
        {
            builder
                .WithJsonConfiguration("config.json", true, TimeSpan.FromMilliseconds(500))
                .WithAppInfoFrom(typeof(App).Assembly)
                .WithLogMinimumLevel<AppHostConfig>(cfg => cfg.LogMinLevel)
                .WithJsonLogFolder<AppHostConfig>("logs", cfg => cfg.RollingSizeKb)
#if DEBUG
                .WithLogToConsole()
#else

#endif
                .WithArguments(args);
        });

        try
        {
            // if you want to use a different Application Delegate class from "AppDelegate"
            // you can specify it here.
            UIApplication.Main(args, null, typeof(AppDelegate));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            host.HandleApplicationCrash(e);
        }
    }
}
