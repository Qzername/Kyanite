using Avalonia;
using HotAvalonia;
using Kyanite.Desktop.NotificationServices;
using System;
using System.Threading;

namespace Kyanite.Desktop;

internal sealed class Program
{
    static readonly string MutexName = "Kyanite";

    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        // Try to create/acquire the mutex
        using var mutex = new Mutex(true, MutexName, out bool isNewInstance);

        if (!isNewInstance)
        {
            NotificationServiceFactory.Create().Show(
                "Kyanite",
                "An instance of this app is already running."
            );

            return;
        }

        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
#if DEBUG
            .UseHotReload()
            .WithDeveloperTools()
#endif
            .WithInterFont()
            .LogToTrace()
            .AfterSetup(ProvideNotificationService);

    static void ProvideNotificationService(AppBuilder builder)
    {
        if (builder.Instance is App app)
            app.RegisterNotificationService(NotificationServiceFactory.Create());
    }
}
