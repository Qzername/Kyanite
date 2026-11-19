using Kyanite.Services;
using System;
using System.Diagnostics;

namespace Kyanite.Desktop.NotificationServices;

internal class LinuxNotificationService : INotificationService
{
    private const string Command = "notify-send";

    public void Show(string title, string message)
    {
        try
        {
            var startInfo = new ProcessStartInfo(Command)
            {
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            startInfo.ArgumentList.Add("--app-name");
            startInfo.ArgumentList.Add("Kyanite");
            startInfo.ArgumentList.Add("--");
            startInfo.ArgumentList.Add(title);
            startInfo.ArgumentList.Add(message);

            using var process = Process.Start(startInfo);
            process?.WaitForExit(500);
        }
        catch (Exception ex)
        {
            Trace.TraceError($"Notification failed: {ex.Message}");
        }
    }
}

