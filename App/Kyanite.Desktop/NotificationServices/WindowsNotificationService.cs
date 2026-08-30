using Kyanite.Services;

#if WINDOWS
using Microsoft.Toolkit.Uwp.Notifications;
#endif

namespace Kyanite.Desktop.NotificationServices;

internal class WindowsNotificationService : INotificationService
{
    public void Show(string title, string message)
    {
#if WINDOWS
        new ToastContentBuilder()
               .AddText(title)
               .AddText(message)
               .Show();
#endif
    }
}
