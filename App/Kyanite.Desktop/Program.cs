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
            new WindowsNotificationService().Show(
                "Kyanite",
                "Instance of this app is already running"
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
        var app = (App)builder.Instance!;

        if (OperatingSystem.IsWindows())
            app.RegisterNotificationService(new WindowsNotificationService());
        else if (OperatingSystem.IsMacOS())
            app.RegisterNotificationService(new MacOsNotificationService());
        else if (OperatingSystem.IsLinux())
            app.RegisterNotificationService(new LinuxNotificationService());
    }
}
