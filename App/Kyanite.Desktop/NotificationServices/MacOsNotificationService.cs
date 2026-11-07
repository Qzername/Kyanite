using Kyanite.Services;
using System;
using System.Diagnostics;

namespace Kyanite.Desktop.NotificationServices;

internal class MacOsNotificationService : INotificationService
{
    public void Show(string title, string message)
    {
        try
        {
            string script = $"display notification {Quote(message)} with title {Quote(title)}";

            var startInfo = new ProcessStartInfo("osascript")
            {
                UseShellExecute = false,
                CreateNoWindow = true
            };

            startInfo.ArgumentList.Add("-e");
            startInfo.ArgumentList.Add(script);

            using var process = Process.Start(startInfo);
        }
        catch (Exception exception)
        {
            Debug.WriteLine($"Could not show a notification: {exception.Message}");
        }
    }

    static string Quote(string value)
        => "\"" + value.Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"";
}
