using Kyanite.Services;
using System;

namespace Kyanite.Desktop.NotificationServices;

internal static class NotificationServiceFactory
{
    public static INotificationService Create()
    {
        if (OperatingSystem.IsWindows())
            return new WindowsNotificationService();

        if (OperatingSystem.IsMacOS())
            return new MacOsNotificationService();

        if (OperatingSystem.IsLinux())
            return new LinuxNotificationService();

        return new NullNotificationService();
    }
}
