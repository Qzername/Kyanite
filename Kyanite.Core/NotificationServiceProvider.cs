namespace Kyanite.Services;

public class NotificationServiceProvider
{
    public INotificationService? ActiveService { get; private set; }

    public void SetService(INotificationService notificationService)
    {
        ActiveService = notificationService;
    }

    public void Show(string title, string message)
    {
        ActiveService?.Show(title, message);
    }
}
