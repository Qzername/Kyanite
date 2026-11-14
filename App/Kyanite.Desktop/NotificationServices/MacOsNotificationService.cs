using Kyanite.Services;

namespace Kyanite.Desktop.NotificationServices;

internal class MacOsNotificationService : INotificationService
{
    // shortcut: no macOS notifications, upgrade with UNUserNotificationCenter when needed; throwing crashed Reminder, ToDo timer and Google Drive save
    public void Show(string title, string message) { }
}
