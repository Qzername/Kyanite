using Kyanite.Services;

namespace Kyanite.Desktop.NotificationServices;

internal class LinuxNotificationService : INotificationService
{
    // shortcut: no Linux notifications, upgrade with libnotify; throwing crashed Reminder, ToDo timer and Google Drive save
    public void Show(string title, string message) { }
}
