using Avalonia;
using Avalonia.ReactiveUI;
using System;
using System.Diagnostics;
using System.Threading;

namespace Whiteboard.Desktop;

class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        var appBuilder = BuildAvaloniaApp();

        appBuilder.AfterSetup((builder) =>
        {
            ((App)builder.Instance!).RegisterNotificationService(new WindowsNotificationService());
        });

        using (var mutex = new Mutex(true, "Whiteboard", out bool createdNew))
        {
            if (!createdNew)
            {
                new WindowsNotificationService().ShowNotification("Whiteboard", "Whiteboard is already running.");  
                return;
            }

            appBuilder.StartWithClassicDesktopLifetime(args);
        }
    }
    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .UseReactiveUI();
}
