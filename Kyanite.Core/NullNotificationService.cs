namespace Kyanite.Services;

/// <summary>
/// Notification service used on platforms that do not expose native notifications.
/// </summary>
public sealed class NullNotificationService : INotificationService
{
    public void Show(string title, string message)
    {
    }
}
