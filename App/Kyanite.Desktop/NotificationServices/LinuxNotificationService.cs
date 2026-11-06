using Kyanite.Services;
using System;
using System.Diagnostics;

namespace Kyanite.Desktop.NotificationServices;

internal class LinuxNotificationService : INotificationService
{
    public void Show(string title, string message)
    {
        try
        {
            var startInfo = new ProcessStartInfo("notify-send")
            {
                UseShellExecute = false,
                CreateNoWindow = true
            };

            startInfo.ArgumentList.Add("--app-name=Kyanite");
            startInfo.ArgumentList.Add("--urgency=normal");
            startInfo.ArgumentList.Add(title);
            startInfo.ArgumentList.Add(message);

            using var process = Process.Start(startInfo);
        }
        catch (Exception exception)
        {
            Debug.WriteLine($"Could not show a notification: {exception.Message}");
        }
    }
}
