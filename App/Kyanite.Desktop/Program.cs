using Avalonia;
using HotAvalonia;
using Kyanite.Desktop.NotificationServices;
using System;

namespace Kyanite.Desktop;

internal sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

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
