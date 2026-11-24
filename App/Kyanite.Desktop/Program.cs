using Avalonia;
using HotAvalonia;
using Kyanite.Core.Services;
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
			CreateNotificationService().Show(
				"Kyanite",
				"Instance of this app is already running"
			);

			return;
		}

		BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
	}

	// Avalonia configuration, don't remove; also used by visual designer.
	public static AppBuilder BuildAvaloniaApp()
	{
		var builder = AppBuilder.Configure<App>()
			.UsePlatformDetect();

		if (OperatingSystem.IsLinux() &&
			Environment.GetEnvironmentVariable("WAYLAND_DISPLAY") is not null)
		{
			builder = builder.UseWayland();
		}

		return builder
#if DEBUG
			.UseHotReload()
			.WithDeveloperTools()
#endif
			.WithInterFont()
			.LogToTrace()
			.AfterSetup(ProvideNotificationService);
	}


	static void ProvideNotificationService(AppBuilder builder)
	{
		var app = (App)builder.Instance!;
		app.RegisterNotificationService(CreateNotificationService());
	}

	static INotificationService CreateNotificationService()
	{
		if (OperatingSystem.IsWindows())
			return new WindowsNotificationService();
		else if (OperatingSystem.IsMacOS())
			return new MacOsNotificationService();
		else if (OperatingSystem.IsLinux())
			return new LinuxNotificationService();
		else
			throw new Exception("Platform not supported");

	}
}
